using Verse;

namespace PostOffice.Audit.Rules.Letters;

internal sealed class LetterDefMatchRule(Func<LetterDef> defSupplier, ChainAction matchAction, Func<PostOfficeSettings, bool> isEnabled, string? debugName = null) 
    : DefMatchRule<Letter, LetterDef>(defSupplier, matchAction, isEnabled, debugName)
{
    protected override LetterDef? GetDefOf(Letter target) => target.def;
}
