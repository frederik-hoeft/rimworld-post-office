using HarmonyLib;
using PostOffice.Audit;
using PostOffice.Audit.Chains;
using PostOffice.Roslyn.Future.ThrowHelpers;
using Verse;

namespace PostOffice.Patching.HarmonyPatches;

[HarmonyPatch(typeof(Messages), nameof(Messages.Message), typeof(Message), typeof(bool))]
public static class Messages_MessagePatch
{
    private static IRuleChain<Message>? s_ruleChain;

    [MemberNotNull(nameof(s_ruleChain))]
    public static void UseRuleChain(IRuleChain<Message> ruleChain)
    {
        Throw.ArgumentNullException.IfNull(ruleChain);
        s_ruleChain = ruleChain;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Method signature must match original method.")]
    public static bool Prefix(Message msg, bool historical = true)
    {
        if (PostOfficeMod.Settings is { isActive: true })
        {
            AssertRuleChain();
            Logger.LogVerbose($"intercepted {nameof(Messages)}.{nameof(Messages.Message)}() using {nameof(Messages_MessagePatch)} for message type '{msg?.def?.defName ?? "<Unknown>"}'.");

            // quick filter, if CanShowInLetterStack is false, then the letter won't be shown anyway
            if (msg is { text: not null and not "" } && s_ruleChain.CanHandle(msg))
            {
                ChainAction action = s_ruleChain.Audit(msg);
                return action is ChainAction.Forward;
            }
        }
        return true;
    }

    [MemberNotNull(nameof(s_ruleChain))]
    private static void AssertRuleChain() =>
        _ = s_ruleChain ?? throw new InvalidOperationException($"[PostOffice] {nameof(LetterStack_ReceiveLetterPatch)} attempted to apply rule chain, but chain was null :C");
}
