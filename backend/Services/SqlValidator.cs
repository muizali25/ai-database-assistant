using System.Text.RegularExpressions;

namespace MiniPosAi.Services;

public static class SqlValidator
{
    public static readonly HashSet<string> AllowedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "Products", "Categories", "Customers", "Sales", "SaleItems"
    };

    private static readonly Regex Forbidden = new(
        @"\b(INSERT|UPDATE|DELETE|MERGE|DROP|ALTER|CREATE|TRUNCATE|EXEC|EXECUTE|GRANT|REVOKE|DENY|INTO|XP_|SP_|OPENROWSET|OPENDATASOURCE|BULK)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex MultipleStatements = new(@";\s*\S", RegexOptions.Compiled);

    private static readonly Regex TableReference = new(
        @"\b(?:FROM|JOIN)\s+(?:\[?dbo\]?\.)?\[?([A-Za-z_][A-Za-z0-9_]*)\]?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public const string OutOfDomainMarker = "OUT_OF_DOMAIN";

    public static bool IsOutOfDomain(string response) =>
        response.Trim().Equals(OutOfDomainMarker, StringComparison.OrdinalIgnoreCase);

    public static bool IsSafeSelect(string sql, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(sql))
        {
            error = "SQL is empty.";
            return false;
        }

        var normalized = sql.Trim().TrimEnd(';');

        if (IsOutOfDomain(normalized))
        {
            error = "Question is outside the Mini POS domain.";
            return false;
        }

        if (!normalized.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
            && !normalized.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
        {
            error = "Only SELECT queries are allowed.";
            return false;
        }

        if (Forbidden.IsMatch(normalized))
        {
            error = "Query contains forbidden keywords. Only read-only SELECT is allowed.";
            return false;
        }

        if (MultipleStatements.IsMatch(normalized))
        {
            error = "Multiple SQL statements are not allowed.";
            return false;
        }

        var tables = TableReference.Matches(normalized)
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (tables.Count == 0)
        {
            error = "Query must reference at least one allowed table.";
            return false;
        }

        var unknown = tables.Where(t => !AllowedTables.Contains(t)).ToList();
        if (unknown.Count > 0)
        {
            error = $"Query references unauthorized tables: {string.Join(", ", unknown)}.";
            return false;
        }

        return true;
    }
}
