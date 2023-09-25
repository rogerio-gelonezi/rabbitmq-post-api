using MessageBus.Engine.Publishers;
using MessagePublisher.WebApi.Properties;
using MessagePublisher.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MessagePublisher.WebApi.Controllers;

[ApiController]
[Route("/controllers/message-publisher")]
public class MessagePublisherController : Controller
{
    private readonly IMessageBusPublisher _messageBusPublisher;

    public MessagePublisherController(IMessageBusPublisher messageBusPublisher)
    {
        _messageBusPublisher = messageBusPublisher;
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(MessagePublisherResponseViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout, Type=typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type=typeof(ErrorResponse))]
    public ActionResult<MessagePublisherResponseViewModel> Publish([FromBody]MessagePublisherRequestViewModel request)
    {
        _messageBusPublisher.Publish(request.Queue, request.Message);
        return new MessagePublisherResponseViewModel(200, Resources.Message_Published_With_Success);
    }
}