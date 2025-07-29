using Verse;

namespace PostOffice.Audit.Rules.Messages;

internal sealed class MessageDefMatchRule(Func<MessageTypeDef> defSupplier, ChainAction matchAction, Func<PostOfficeSettings, bool> isEnabled, string? debugName = null) 
    : DefMatchRule<Message, MessageTypeDef>(defSupplier, matchAction, isEnabled, debugName)
{
    protected override MessageTypeDef? GetDefOf(Message target) => target.def;
}
