using System.ComponentModel.DataAnnotations;

namespace MessagePublisher.WebApi.ViewModels;

public class MessagePublisherRequestViewModel
{
    public MessagePublisherRequestViewModel(string queue, string message)
    {
        Queue = queue;
        Message = message;
    }

    [Required]
    public string Queue { get; }
    [Required]
    public string Message { get; }
}