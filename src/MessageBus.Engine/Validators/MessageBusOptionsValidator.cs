using MessageBus.Engine.MessageBus;
using MessageBus.Engine.Properties;

namespace MessageBus.Engine.Validators;

internal static class MessageBusOptionsValidator
{
    public static void Validate(MessageBusOptions options)
    {
        ValidateUsername(options);
        ValidatePassword(options);
        ValidateHostname(options);
    }

    private static void ValidateHostname(MessageBusOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException(Resources.Validator_Options_Hostname);
    }

    private static void ValidateUsername(MessageBusOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Username))
            throw new InvalidOperationException(Resources.Validator_Options_Username);
    }

    private static void ValidatePassword(MessageBusOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HostName))
            throw new InvalidOperationException(Resources.Validator_Options_Password);
    }
}