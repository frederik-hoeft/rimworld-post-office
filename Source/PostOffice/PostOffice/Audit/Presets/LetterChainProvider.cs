using PostOffice.Audit.Chains;
using PostOffice.Audit.Rules.Letters;
using PostOffice.Dependencies.BuiltIn;
using RimWorld;
using System.Text.RegularExpressions;
using Verse;

namespace PostOffice.Audit.Presets;

internal static class LetterChainProvider
{
    public static IRuleChain<Letter> GetChain()
    {
        IRuleChain<Letter> chain = new LetterRuleChain
        {
            DefaultAction = ChainAction.Forward
        };
        // Core LetterDefs
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.AcceptJoiner, static settings => settings.dropAcceptJoiner, nameof(LetterDefOf.AcceptJoiner)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.AcceptVisitors, static settings => settings.dropAcceptVisitors, nameof(LetterDefOf.AcceptVisitors)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.BundleLetter, static settings => settings.dropBundleLetter, nameof(LetterDefOf.BundleLetter)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.ChoosePawn, static settings => settings.dropChoosePawn, nameof(LetterDefOf.ChoosePawn)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.Death, static settings => settings.dropDeath, nameof(LetterDefOf.Death)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.NegativeEvent, static settings => settings.dropNegativeEvent, nameof(LetterDefOf.NegativeEvent)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.NeutralEvent, static settings => settings.dropNeutralEvent, nameof(LetterDefOf.NeutralEvent)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.PositiveEvent, static settings => settings.dropPositiveEvent, nameof(LetterDefOf.PositiveEvent)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.RitualOutcomeNegative, static settings => settings.dropRitualOutcomeNegative, nameof(LetterDefOf.RitualOutcomeNegative)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.RitualOutcomePositive, static settings => settings.dropRitualOutcomePositive, nameof(LetterDefOf.RitualOutcomePositive)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.ThreatBig, static settings => settings.dropThreatBig, nameof(LetterDefOf.ThreatBig)));
        chain.Add(LetterRule.DropIfMatches(static () => LetterDefOf.ThreatSmall, static settings => settings.dropThreatSmall, nameof(LetterDefOf.ThreatSmall)));
        // Ideology LetterDefs
        chain.Add<RequiresIdeology>(LetterRule.DropIfMatches(static () => LetterDefOf.RelicHuntInstallationFound, settings => settings.dropRelicHuntInstallationFound, nameof(LetterDefOf.RelicHuntInstallationFound)));
        // Biotech LetterDefs
        chain.Add<RequiresBiotech>(LetterRule.DropIfMatches(static () => LetterDefOf.BabyBirth, static settings => settings.dropBabyBirth, nameof(LetterDefOf.BabyBirth)));
        chain.Add<RequiresBiotech>(LetterRule.DropIfMatches(static () => LetterDefOf.BabyToChild, static settings => settings.dropBabyToChild, nameof(LetterDefOf.BabyToChild)));
        chain.Add<RequiresBiotech>(LetterRule.DropIfMatches(static () => LetterDefOf.Bossgroup, static settings => settings.dropBossgroup, nameof(LetterDefOf.Bossgroup)));
        chain.Add<RequiresBiotech>(LetterRule.DropIfMatches(static () => LetterDefOf.ChildBirthday, static settings => settings.dropChildBirthday, nameof(LetterDefOf.ChildBirthday)));
        chain.Add<RequiresBiotech>(LetterRule.DropIfMatches(static () => LetterDefOf.ChildToAdult, static settings => settings.dropChildToAdult, nameof(LetterDefOf.ChildToAdult)));
        // Anomaly LetterDefs
        chain.Add<RequiresAnomaly>(LetterRule.DropIfMatches(static () => LetterDefOf.AcceptCreepJoiner, static settings => settings.dropAcceptCreepJoiner, nameof(LetterDefOf.AcceptCreepJoiner)));
        chain.Add<RequiresAnomaly>(LetterRule.DropIfMatches(static () => LetterDefOf.EntityDiscovered, static settings => settings.dropEntityDiscovered, nameof(LetterDefOf.EntityDiscovered)));
        // dynamic
        chain.Add(LetterRule.Dynamic(static letter => (letter.Label.RawText, PostOfficeMod.Settings.DropRegexCompiled) switch
        {
            (string label, Regex validRegex) when validRegex.IsMatch(label) => ChainAction.Drop,
            _ => ChainAction.NextHandler
        }, settings => settings.DropRegex is not null, "DropByRegex"));
        return chain;
    }
}
