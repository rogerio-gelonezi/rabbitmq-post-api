namespace MessagePublisher.WebApi.ViewModels;

public class ErrorResponse
{
    public ErrorResponse() { }

    public ErrorResponse(int code, string title, string message)
    {
        Code = code;
        Title = title;
        Message = message;
    }
    
    public int? Code { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public ErrorResponse? InnerError { get; set; }
    public string[]? Details { get; set; }
    public string? TraceId { get; set; }
}