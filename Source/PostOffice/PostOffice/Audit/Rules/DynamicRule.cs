namespace PostOffice.Audit.Rules;

internal class DynamicRule<TTarget> : OptionalRule<TTarget>
{
    private protected Func<TTarget, ChainAction> RuleDefinition { get; }

    internal protected DynamicRule(Func<TTarget, ChainAction> ruleDefinition, Func<PostOfficeSettings, bool> isEnabled, string? debugName = null) 
        : base(isEnabled, debugName) => RuleDefinition = ruleDefinition;

    public override ChainAction Audit(TTarget target) => 
        RuleDefinition.Invoke(target);
}
