using Verse;

namespace PostOffice.Audit.Rules.Letters;

internal sealed class LetterRule : RuleProvider
{
    private LetterRule() { }

    public static IRule<Letter> Dynamic(Func<Letter, ChainAction> ruleDefinition, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new DynamicRule<Letter>(letter => ruleDefinition(letter), isEnabled ?? True, debugName);

    public static IRule<Letter> DropIfMatches(Func<LetterDef> defSupplier, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new LetterDefMatchRule(defSupplier, ChainAction.Drop, isEnabled ?? True, debugName);

    public static IRule<Letter> ForwardIfMatches(Func<LetterDef> defSupplier, Func<PostOfficeSettings, bool>? isEnabled = null, string? debugName = null) =>
        new LetterDefMatchRule(defSupplier, ChainAction.Forward, isEnabled ?? True, debugName);
}
