using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MiniPosAi.Services;

public class GeminiService(HttpClient httpClient, IConfiguration configuration)
{
    private const string SchemaDescription = """
        Database schema (SQL Server):

        Categories (
          CategoryId INT PRIMARY KEY,
          Name NVARCHAR(100) NOT NULL,
          Description NVARCHAR(250) NULL
        )

        Products (
          ProductId INT PRIMARY KEY,
          CategoryId INT NOT NULL REFERENCES Categories(CategoryId),
          Name NVARCHAR(150) NOT NULL,
          Price DECIMAL(18,2) NOT NULL,
          Stock INT NOT NULL
        )

        Customers (
          CustomerId INT PRIMARY KEY,
          Name NVARCHAR(150) NOT NULL,
          Email NVARCHAR(150) NOT NULL,
          Phone NVARCHAR(30) NULL
        )

        Sales (
          SaleId INT PRIMARY KEY,
          CustomerId INT NOT NULL REFERENCES Customers(CustomerId),
          SaleDate DATETIME2 NOT NULL,
          TotalAmount DECIMAL(18,2) NOT NULL
        )

        SaleItems (
          SaleItemId INT PRIMARY KEY,
          SaleId INT NOT NULL REFERENCES Sales(SaleId),
          ProductId INT NOT NULL REFERENCES Products(ProductId),
          Quantity INT NOT NULL,
          UnitPrice DECIMAL(18,2) NOT NULL
        )
        """;

    public async Task<string> GenerateSqlAsync(string question, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Gemini:ApiKey is missing. Set it in appsettings or user secrets.");

        var model = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var prompt = $"""
            You are a SQL assistant for a Mini POS (point of sale) database only.
            Convert the user's question into a single read-only T-SQL SELECT query.

            Rules:
            - If the question is NOT about this Mini POS data (products, categories, customers, sales, sale items),
              reply with exactly: {SqlValidator.OutOfDomainMarker}
            - Do not invent other domains (weather, news, HR, finance outside POS, etc.).
            - Return ONLY the SQL query OR the marker. No markdown, no explanation, no code fences.
            - Use ONLY SELECT statements against these tables: Products, Categories, Customers, Sales, SaleItems.
            - Never use INSERT, UPDATE, DELETE, MERGE, DROP, ALTER, CREATE, TRUNCATE, EXEC, or INTO.
            - Prefer joining related tables when useful.
            - Use the exact table and column names from the schema.

            {SchemaDescription}

            User question: {question}
            """;

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Gemini API error ({(int)response.StatusCode}): {body}");

        using var doc = JsonDocument.Parse(body);
        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Gemini returned an empty response.");

        return CleanSql(text);
    }

    private static string CleanSql(string raw)
    {
        var sql = raw.Trim();
        sql = Regex.Replace(sql, @"^```(?:sql)?\s*", "", RegexOptions.IgnoreCase);
        sql = Regex.Replace(sql, @"\s*```$", "", RegexOptions.IgnoreCase);
        return sql.Trim().TrimEnd(';');
    }
}
