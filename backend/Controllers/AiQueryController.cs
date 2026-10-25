using Microsoft.AspNetCore.Mvc;
using MiniPosAi.Services;

namespace MiniPosAi.Controllers;

[ApiController] 
[Route("api/ai")]
public class AiQueryController(GeminiService gemini, QueryExecutor executor) : ControllerBase
{
    public record AskRequest(string Question);

    /// <summary>
    /// Generate SQL via Gemini, validate (SELECT + allowed tables only), then execute.
    /// No client approval step. Out-of-domain / unsafe queries return "Data not found".
    /// </summary>
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { found = false, message = "Question is required." });

        try
        {
            var generated = await gemini.GenerateSqlAsync(request.Question.Trim(), cancellationToken);

            if (SqlValidator.IsOutOfDomain(generated) || !SqlValidator.IsSafeSelect(generated, out _))
            {
                return Ok(new
                {
                    found = false,
                    message = "Data not found",
                    data = Array.Empty<object>()
                });
            }

            var rows = await executor.ExecuteSelectAsync(generated, cancellationToken);

            if (rows.Count == 0)
            {
                return Ok(new
                {
                    found = false,
                    message = "Data not found",
                    data = Array.Empty<object>()
                });
            }

            return Ok(new
            {
                found = true,
                message = "OK",
                rowCount = rows.Count,
                data = rows
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                found = false,
                message = ex.Message,//"Data not found",
                data = Array.Empty<object>()
            });
        }
    }
}
