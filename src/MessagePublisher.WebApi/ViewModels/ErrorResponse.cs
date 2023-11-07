namespace MessagePublisher.WebApi.ViewModels;

public class ErrorResponse
{
    public int? Code { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public ErrorResponse? InnerError { get; set; }
    public string[]? Details { get; set; }
    public string? TraceId { get; set; }
}