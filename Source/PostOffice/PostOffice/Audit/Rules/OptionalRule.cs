namespace PostOffice.Audit.Rules;

internal abstract class OptionalRule<TTarget>(Func<PostOfficeSettings, bool> isEnabled, string? debugName = null) : BaseRule<TTarget>(debugName)
{
    private readonly Func<PostOfficeSettings, bool> _isEnabled = isEnabled;

    public override bool CanHandle(TTarget target) =>
        _isEnabled.Invoke(PostOfficeMod.Settings);
}
