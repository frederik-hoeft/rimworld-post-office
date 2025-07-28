using Verse;

namespace PostOffice.Audit.Rules.Messages;

internal sealed class MessageRule : RuleProvider
{
    private MessageRule() { }

    public static IRule<Message> Dynamic(Func<Message, ChainAction> ruleDefinition, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new DynamicRule<Message>(message => ruleDefinition(message), isEnabled ?? True, debugName);

    public static IRule<Message> DropIfMatches(Func<MessageTypeDef> defSupplier, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new MessageDefMatchRule(defSupplier, ChainAction.Drop, isEnabled ?? True, debugName);

    public static IRule<Message> ForwardIfMatches(Func<MessageTypeDef> defSupplier, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new MessageDefMatchRule(defSupplier, ChainAction.Forward, isEnabled ?? True, debugName);
}
