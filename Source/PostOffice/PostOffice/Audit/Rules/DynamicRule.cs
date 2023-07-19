﻿using Verse;

namespace PostOffice.Audit.Rules;

internal class DynamicRule : OptionalRule
{
    private readonly Func<Letter, MessageAction> _ruleDefinition;

    internal DynamicRule(Func<Letter, MessageAction> ruleDefinition, Func<PostOfficeSettings, bool> isEnabled, string? debugName = null) : base(isEnabled, debugName) => 
        _ruleDefinition = ruleDefinition;

    public override MessageAction Audit(Letter letter) => 
        _ruleDefinition.Invoke(letter);
}