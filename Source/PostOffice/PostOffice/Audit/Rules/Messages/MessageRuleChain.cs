using PostOffice.Audit.Chains;
using System.Collections.Generic;
using Verse;

namespace PostOffice.Audit.Rules.Messages;

internal sealed class MessageRuleChain(List<IRule<Message>> rules) : RuleChain<Message>(rules)
{
    public MessageRuleChain() : this([]) { }

    protected override string TargetTypeName => nameof(Message);

    public override bool CanHandle(Message target) => true;
}
