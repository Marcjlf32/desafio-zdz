namespace Catalog.Tests;

public class ProblemDetailsResponse
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
}

public class ValidationProblemResponse : ProblemDetailsResponse
{
    public Dictionary<string, string[]>? Errors { get; set; }
}
