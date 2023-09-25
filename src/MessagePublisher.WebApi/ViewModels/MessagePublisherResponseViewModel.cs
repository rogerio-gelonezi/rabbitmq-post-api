namespace MessagePublisher.WebApi.ViewModels;

public class MessagePublisherResponseViewModel
{
    public MessagePublisherResponseViewModel(int status, string message)
    {
        Status = status;
        Message = message;
    }

    public int Status { get; }
    public string Message { get; }
}