using HarmonyLib;
using PostOffice.Roslyn.Future.ThrowHelpers;
using PostOffice.Audit;
using PostOffice.Audit.Chains;
using Verse;

namespace PostOffice.Patching.HarmonyPatches;

[HarmonyPatch(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), typeof(Letter), typeof(string), typeof(int), typeof(bool))]
public static class LetterStack_ReceiveLetterPatch
{
    private static IRuleChain<Letter>? s_ruleChain;

    [MemberNotNull(nameof(s_ruleChain))]
    public static void UseRuleChain(IRuleChain<Letter> ruleChain)
    {
        Throw.ArgumentNullException.IfNull(ruleChain);
        s_ruleChain = ruleChain;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Method signature must match original method.")]
    public static bool Prefix(Letter let, string? debugInfo = null, int delayTicks = 0, bool playSound = true)
    {
        if (PostOfficeMod.Settings is { isActive: true })
        {
            AssertRuleChain();
            Logger.LogVerbose($"intercepted {nameof(LetterStack)}.{nameof(LetterStack.ReceiveLetter)}() using {nameof(LetterStack_ReceiveLetterPatch)} for letter type '{let?.def?.defName ?? "<Unknown>"}'.");

            // quick filter, if CanShowInLetterStack is false, then the letter won't be shown anyway
            if (let is { CanShowInLetterStack: true } && s_ruleChain.CanHandle(let))
            {
                ChainAction action = s_ruleChain.Audit(let);
                return action is ChainAction.Forward;
            }
        }
        return true;
    }

    [MemberNotNull(nameof(s_ruleChain))]
    private static void AssertRuleChain() =>
        _ = s_ruleChain ?? throw new InvalidOperationException($"[PostOffice] {nameof(LetterStack_ReceiveLetterPatch)} attempted to apply rule chain, but chain was null :C");
}