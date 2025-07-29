using PostOffice.Audit.Chains;
using System.Collections.Generic;
using Verse;

namespace PostOffice.Audit.Rules.Letters;

internal sealed class LetterRuleChain(List<IRule<Letter>> rules) : RuleChain<Letter>(rules)
{
    public LetterRuleChain() : this([]) { }

    protected override string TargetTypeName => nameof(Letter);

    public override bool CanHandle(Letter target) => true;
}
