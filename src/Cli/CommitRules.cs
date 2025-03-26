using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommitRules
{
    /// <summary>
    /// Rules match the input either as successful or failed.
    /// For example, when `header-full-stop` detects a full stop and is set as "always"; it's true.
    /// If the `header-full-stop` discovers a full stop but is set to "never"; it's false.
    /// </summary>
    public readonly record struct RuleOutcome(bool IsSuccessful, string? Message = null);

    /// <summary>
    /// Rules receive a parsed commit, condition, and possible additional settings through value.
    /// All rules should provide the most sensible rule condition and value.
    /// </summary>
    public enum RuleType { Async, Sync, Either }

    /// <summary>
    /// Represents a commit as parsed by conventional-commits-parser
    /// (Note: In a real implementation, you'd define the full Commit class)
    /// </summary>
    public class Commit { }

    /// <summary>
    /// Rules always have a severity.
    /// Severity indicates what to do if the rule is found to be broken
    /// 0 - Disable this rule
    /// 1 - Warn for violations
    /// 2 - Error for violations
    /// </summary>
    public enum RuleConfigSeverity { Disabled = 0, Warning = 1, Error = 2 }

    /// <summary>
    /// Rules always have a condition.
    /// It can be either "always" (as tested), or "never" (as tested).
    /// For example, `header-full-stop` can be enforced as "always" or "never".
    /// </summary>
    public enum RuleConfigCondition { Always, Never }

    /// <summary>
    /// Delegate types for different rule implementations
    /// </summary>
    public delegate RuleOutcome SyncRuleDelegate<TValue>(Commit parsed, RuleConfigCondition? when = null, TValue? value = default);
    public delegate Task<RuleOutcome> AsyncRuleDelegate<TValue>(Commit parsed, RuleConfigCondition? when = null, TValue? value = default);

    /// <summary>
    /// Represents the quality of rule configuration
    /// </summary>
    public enum RuleConfigQuality { User, Qualified }

    /// <summary>
    /// Rule configuration with support for various value types
    /// </summary>
    public record RuleConfig<TValue>(RuleConfigSeverity Severity, RuleConfigCondition? Condition = null, TValue? Value = default);

    /// <summary>
    /// Specific rule configuration types
    /// </summary>
    public record CaseRuleConfig(RuleConfigSeverity Severity, RuleConfigCondition? Condition = null, string[]? Value = default)
        : RuleConfig<string[]>(Severity, Condition, Value);
    public record LengthRuleConfig(RuleConfigSeverity Severity, RuleConfigCondition? Condition = null, int Value = default)
        : RuleConfig<int>(Severity, Condition, Value);
    public record EnumRuleConfig(RuleConfigSeverity Severity, RuleConfigCondition? Condition = null, string[]? Value = default)
        : RuleConfig<string[]>(Severity, Condition, Value);

    /// <summary>
    /// Comprehensive rules configuration
    /// </summary>
    public class RulesConfig
    {
        public RuleConfig<string[]> BodyCase { get; set; }
        public RuleConfig<object> BodyEmpty { get; set; }
        public RuleConfig<string> BodyFullStop { get; set; }
        public RuleConfig<object> BodyLeadingBlank { get; set; }
        public LengthRuleConfig BodyMaxLength { get; set; }
        public LengthRuleConfig BodyMaxLineLength { get; set; }
        public LengthRuleConfig BodyMinLength { get; set; }

        // ... (continue with other rules similar to the TypeScript version)

        // Allow for custom plugin rules
        public Dictionary<string, object> CustomRules { get; set; } = new Dictionary<string, object>();
    }
}