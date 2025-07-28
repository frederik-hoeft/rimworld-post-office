// shims for modern Roslyn features in legacy .NET Framework

namespace System.Runtime.CompilerServices;

public sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}
