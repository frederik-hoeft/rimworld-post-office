using HarmonyLib;
using PostOffice.Audit.Chains;
using PostOffice.Audit.Presets;
using PostOffice.Dependencies.BuiltIn;
using PostOffice.Patching;
using PostOffice.Patching.HarmonyPatches;
using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace PostOffice;

public sealed class PostOfficeMod : Mod
{
    public static PostOfficeSettings Settings { get; private set; } = null!;

    public PostOfficeMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<PostOfficeSettings>();

        IRuleChain<Letter> letterChain = LetterChainProvider.GetChain();
        LetterStack_ReceiveLetterPatch.UseRuleChain(letterChain);

        IRuleChain<Message> messageChain = MessageChainProvider.GetChain();
        Messages_MessagePatch.UseRuleChain(messageChain);

        PostOfficePatches.Apply(new Harmony("Th3Fr3d.PostOffice"));
    }

    private Vector2 _scrollPosition;
    private const float MIN_CONTENT_HEIGHT = 256f;
    private float _knownContentHeight = MIN_CONTENT_HEIGHT;
    private bool _requiresScrolling = false;

    public override void DoSettingsWindowContents(Rect canvas)
    {
        base.DoSettingsWindowContents(canvas);
        if (Settings is null)
        {
            Logger.Error("Settings was null!");
            throw new ArgumentNullException(nameof(Settings));
        }
        float scrollbarMargin = _requiresScrolling ? 25f : 0f;
        Listing_Standard list = new()
        {
            ColumnWidth = (canvas.width - scrollbarMargin - 17) / 2,
        };
        Rect content = new(canvas.x, canvas.y, canvas.width - scrollbarMargin, _knownContentHeight);
        Rect view = new(canvas.x, canvas.y, canvas.width, canvas.height);
        if (_requiresScrolling)
        {
            Widgets.BeginScrollView(view, ref _scrollPosition, content);
        }
        list.Begin(content);
        Text.Font = GameFont.Medium;
        list.Label("General settings");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Is active", ref Settings.isActive);
        list.CheckboxLabeled("Enable message support", ref Settings.enableMessageSupport, 
            "Also prevents messages in the top left from being shown if their attributes match the provided filters");
        list.CheckboxLabeled("Enable logging", ref Settings.enableLogging);
        list.CheckboxLabeled("Enable verbose logging", ref Settings.enableVerboseLogging);
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Mod support");
        Text.Font = GameFont.Small;
        if (ModDependency.IsCai5000Loaded)
        {
            list.CheckboxLabeled("CAI-5000 Fog of War", ref Settings.cai5000_delayCombatMusic,
                $"If 'CAI 5000 - Advanced AI + Fog Of War' is loaded, {SettingsCategory()} will delay combat music until hostile forces have been detected by your colonists.");
        }
        list.NewColumn();
        Text.Font = GameFont.Medium;
        list.Label("Rules");
        list.GapLine();
        list.Label("RimWorld");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.AcceptJoiner))}'", ref Settings.dropAcceptJoiner);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.AcceptVisitors))}'", ref Settings.dropAcceptVisitors);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.BundleLetter))}'", ref Settings.dropBundleLetter);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.ChoosePawn))}'", ref Settings.dropChoosePawn);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.Death))}'", ref Settings.dropDeath);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.NegativeEvent))}'", ref Settings.dropNegativeEvent);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.NeutralEvent))}'", ref Settings.dropNeutralEvent);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.PositiveEvent))}'", ref Settings.dropPositiveEvent);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.RitualOutcomeNegative))}'", ref Settings.dropRitualOutcomeNegative);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.RitualOutcomePositive))}'", ref Settings.dropRitualOutcomePositive);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.ThreatBig))}'", ref Settings.dropThreatBig);
        list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.ThreatSmall))}'", ref Settings.dropThreatSmall);
        if (ModDependency.IsSatisfied<RequiresIdeology>())
        {
            list.GapLine();
            Text.Font = GameFont.Medium;
            list.Label("Ideology");
            Text.Font = GameFont.Small;
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.RelicHuntInstallationFound))}'", ref Settings.dropRelicHuntInstallationFound);
        }
        if (ModDependency.IsSatisfied<RequiresBiotech>())
        {
            list.GapLine();
            Text.Font = GameFont.Medium;
            list.Label("Biotech");
            Text.Font = GameFont.Small;
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.BabyBirth))}'", ref Settings.dropBabyBirth);
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.BabyToChild))}'", ref Settings.dropBabyToChild);
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.Bossgroup))}'", ref Settings.dropBossgroup);
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.ChildBirthday))}'", ref Settings.dropChildBirthday);
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.ChildToAdult))}'", ref Settings.dropChildToAdult);
        }
        if (ModDependency.IsSatisfied<RequiresAnomaly>())
        {
            list.GapLine();
            Text.Font = GameFont.Medium;
            list.Label("Anomaly");
            Text.Font = GameFont.Small;
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.AcceptCreepJoiner))}'", ref Settings.dropAcceptCreepJoiner);
            list.CheckboxLabeled($"Hide letters of type '{PascalCaseToDisplayString(nameof(LetterDefOf.EntityDiscovered))}'", ref Settings.dropEntityDiscovered);
        }
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Advanced");
        Text.Font = GameFont.Small;
        list.Label("Hide letters by regular expression:");
        Settings.DropRegex = list.TextEntry(Settings.DropRegex);
        if (!Settings.DropRegexIsValid)
        {
            Text.Font = GameFont.Tiny;
            list.Label($"Invalid pattern: {Settings.DropRegexLatestError}", tooltip: "The regular expression could not be compiled. Please check the syntax (.NET regular expressions) and try again.");
        }
        if (Settings.enableMessageSupport)
        {
            list.Label("Hide messages (top left) by regular expression:");
            Settings.DropMessageRegex = list.TextEntry(Settings.DropMessageRegex);
            if (!Settings.DropMessageRegexIsValid)
            {
                Text.Font = GameFont.Tiny;
                list.Label($"Invalid pattern: {Settings.DropMessageRegexLatestError}", tooltip: "The regular expression could not be compiled. Please check the syntax (.NET regular expressions) and try again.");
            }
        }
        // FIXME: my god, this absolutely sucks... :P
        // we basically do an initial draw with a rather small height, and see if it's enough
        // - if it is, we remember the exact height of the content and resize the window to fit
        // - otherwise CurHeight will be smaller than a known epsilon, so we start at the epsilon and double it with each draw until it's big enough
        // - once we have enough space, we clamp the height to the known content height
        // if the content is too small to fit everything, CurHeight will be 23.something, (don't ask me why)
        if (list.CurHeight < MIN_CONTENT_HEIGHT)
        {
            // so on each draw, we double the known content height until it's big enough
            // yes, we could also just set it to an unreasonably high value, but that seems a bit wasteful
            // we should arrive at the correct height in O(log n) iterations anyway
            _knownContentHeight = Mathf.Max(2 * _knownContentHeight, MIN_CONTENT_HEIGHT);
        }
        else
        {
            // when we have enough space, we remember the height
            _knownContentHeight = list.CurHeight;
        }
        list.End();
        bool requiresScrolling = _knownContentHeight > canvas.height;
        if (_requiresScrolling)
        {
            Widgets.EndScrollView();
        }
        _requiresScrolling = requiresScrolling;
        base.DoSettingsWindowContents(canvas);
    }

    public override string SettingsCategory() => "Post Office";

    private static string PascalCaseToDisplayString(string pascalCase)
    {
        StringBuilder bobTheBuilder = new();
        for (int i = 0; i < pascalCase.Length; i++)
        {
            if (i > 0 && char.IsUpper(pascalCase[i]))
            {
                bobTheBuilder.Append(' ');
            }
            bobTheBuilder.Append(pascalCase[i]);
        }
        return bobTheBuilder.ToString();
    }
}
