using PostOffice.Audit.Chains;
using PostOffice.Audit.Rules.Messages;
using RimWorld;
using System.Text.RegularExpressions;
using Verse;

namespace PostOffice.Audit.Presets;

internal static class MessageChainProvider
{
    public static IRuleChain<Message> GetChain()
    {
        IRuleChain<Message> chain = new MessageRuleChain
        {
            DefaultAction = ChainAction.Forward
        };
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.ThreatBig, static settings => settings.dropThreatBig, nameof(MessageTypeDefOf.ThreatBig)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.ThreatSmall, static settings => settings.dropThreatSmall, nameof(MessageTypeDefOf.ThreatSmall)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.NegativeEvent, static settings => settings.dropNegativeEvent, nameof(MessageTypeDefOf.NegativeEvent)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.NegativeHealthEvent, static settings => settings.dropNegativeEvent, nameof(MessageTypeDefOf.NegativeHealthEvent)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.NeutralEvent, static settings => settings.dropNeutralEvent, nameof(MessageTypeDefOf.NeutralEvent)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.PawnDeath, static settings => settings.dropDeath, nameof(MessageTypeDefOf.PawnDeath)));
        chain.Add(MessageRule.DropIfMatches(static () => MessageTypeDefOf.PositiveEvent, static settings => settings.dropPositiveEvent, nameof(MessageTypeDefOf.PositiveEvent)));
        // dynamic
        chain.Add(MessageRule.Dynamic(static message => (message.text, PostOfficeMod.Settings.DropMessageRegexCompiled) switch
        {
            (string label, Regex validRegex) when validRegex.IsMatch(label) => ChainAction.Drop,
            _ => ChainAction.NextHandler
        }, settings => settings.DropRegex is not null, "DropByMessageRegex"));
        return chain;
    }
}
