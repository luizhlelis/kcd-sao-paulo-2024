using DotNetCore.CAP.Filter;
using DotNetCore.CAP.Messages;

namespace Catalog.Consumer.Idempotency;

public class BootstrapFilter : SubscribeFilter
{
    public override Task OnSubscribeExecutingAsync(ExecutingContext context)
    {
        var message = context.Arguments
            .FirstOrDefault(x => x is IMessage);
        if (message is null)
            throw new InvalidOperationException("Message must be of type IMessage");
        ((IMessage)message).MessageId = context.DeliverMessage.GetId();
        ((IMessage)message).MessageGroup = context.DeliverMessage.GetGroup() ?? string.Empty;

        return Task.CompletedTask;
    }
}
