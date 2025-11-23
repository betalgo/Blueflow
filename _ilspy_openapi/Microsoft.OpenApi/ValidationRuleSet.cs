using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.OpenApi;

/// <summary>
/// The rule set of the validation.
/// </summary>
public sealed class ValidationRuleSet
{
	private Dictionary<Type, List<ValidationRule>> _rulesDictionary = new Dictionary<Type, List<ValidationRule>>();

	private static ValidationRuleSet? _defaultRuleSet;

	private readonly List<ValidationRule> _emptyRules = new List<ValidationRule>();

	/// <summary>
	/// Gets the rules in this rule set.
	/// </summary>
	public IList<ValidationRule> Rules => _rulesDictionary.Values.SelectMany((List<ValidationRule> v) => v).ToList();

	/// <summary>
	/// Gets the number of elements contained in this rule set.
	/// </summary>
	public int Count => _rulesDictionary.Count;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.ValidationRuleSet" /> class.
	/// </summary>
	public ValidationRuleSet()
	{
	}

	/// <summary>
	/// Retrieve the rules that are related to a specific type
	/// </summary>
	/// <param name="type">The type that is to be validated</param>
	/// <returns>Either the rules related to the type, or an empty list.</returns>
	public IList<ValidationRule> FindRules(Type type)
	{
		_rulesDictionary.TryGetValue(type, out List<ValidationRule> value);
		return value ?? _emptyRules;
	}

	/// <summary>
	/// Gets the default validation rule sets.
	/// </summary>
	/// <remarks>
	/// This is a method instead of a property to signal that a new default rule-set object is created
	/// per call. Making this a property may be misleading callers to think the returned rule-sets from multiple calls
	/// are the same objects.
	/// </remarks>
	public static ValidationRuleSet GetDefaultRuleSet()
	{
		if (_defaultRuleSet == null)
		{
			_defaultRuleSet = BuildDefaultRuleSet();
		}
		return new ValidationRuleSet(_defaultRuleSet);
	}

	/// <summary>
	/// Return <see cref="T:Microsoft.OpenApi.ValidationRuleSet" /> with no rules
	/// </summary>
	public static ValidationRuleSet GetEmptyRuleSet()
	{
		return new ValidationRuleSet();
	}

	/// <summary>
	/// Add validation rules to the rule set.
	/// </summary>
	/// <param name="ruleSet">The rule set to add validation rules to.</param>
	/// <param name="rules">The validation rules to be added to the rules set.</param>
	/// <exception cref="T:Microsoft.OpenApi.OpenApiException">Throws a null argument exception if the arguments are null.</exception>
	public static void AddValidationRules(ValidationRuleSet ruleSet, Dictionary<Type, List<ValidationRule>> rules)
	{
		if (ruleSet == null || rules == null)
		{
			throw new OpenApiException(SRResource.ArgumentNull);
		}
		foreach (KeyValuePair<Type, List<ValidationRule>> rule in rules)
		{
			ruleSet.Add(rule.Key, rule.Value);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.ValidationRuleSet" /> class.
	/// </summary>
	/// <param name="ruleSet">Rule set to be copied from.</param>
	public ValidationRuleSet(ValidationRuleSet ruleSet)
	{
		if (ruleSet == null)
		{
			return;
		}
		foreach (ValidationRule item in ruleSet)
		{
			Add(item.ElementType, item);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.ValidationRuleSet" /> class.
	/// </summary>
	/// <param name="rules">Rules to be contained in this ruleset.</param>
	public ValidationRuleSet(Dictionary<Type, List<ValidationRule>> rules)
	{
		if (rules == null)
		{
			return;
		}
		foreach (KeyValuePair<Type, List<ValidationRule>> rule in rules)
		{
			Add(rule.Key, rule.Value);
		}
	}

	/// <summary>
	/// Add the new rule into the rule set.
	/// </summary>
	/// <param name="key">The key for the rule.</param>
	/// <param name="rules">The list of rules.</param>
	public void Add(Type key, List<ValidationRule> rules)
	{
		foreach (ValidationRule rule in rules)
		{
			Add(key, rule);
		}
	}

	/// <summary>
	/// Add a new rule into the rule set.
	/// </summary>
	/// <param name="key">The key for the rule.</param>
	/// <param name="rule">The rule.</param>
	/// <exception cref="T:Microsoft.OpenApi.OpenApiException">Exception thrown when rule already exists.</exception>
	public void Add(Type key, ValidationRule rule)
	{
		if (!_rulesDictionary.TryGetValue(key, out List<ValidationRule> value))
		{
			value = (_rulesDictionary[key] = new List<ValidationRule>());
		}
		if (value.Contains(rule))
		{
			throw new OpenApiException(SRResource.Validation_RuleAddTwice);
		}
		_rulesDictionary[key].Add(rule);
	}

	/// <summary>
	/// Updates an existing rule with a new one.
	/// </summary>
	/// <param name="key">The key of the existing rule.</param>
	/// <param name="newRule">The new rule.</param>
	/// <param name="oldRule">The old rule.</param>
	/// <returns>true, if the update was successful; otherwise false.</returns>
	public bool Update(Type key, ValidationRule newRule, ValidationRule oldRule)
	{
		if (_rulesDictionary.TryGetValue(key, out List<ValidationRule> value))
		{
			value.Add(newRule);
			return value.Remove(oldRule);
		}
		return false;
	}

	/// <summary>
	/// Removes a collection of rules.
	/// </summary>
	/// <param name="key">The key of the collection of rules to be removed.</param>
	/// <returns>true if the collection of rules with the provided key is removed; otherwise, false.</returns>
	public bool Remove(Type key)
	{
		return _rulesDictionary.Remove(key);
	}

	/// <summary>
	/// Remove a rule by its name from all types it is used by.
	/// </summary>        
	/// <param name="ruleName">Name of the rule.</param>
	public void Remove(string ruleName)
	{
		foreach (KeyValuePair<Type, List<ValidationRule>> item in _rulesDictionary)
		{
			_rulesDictionary[item.Key] = item.Value.Where((ValidationRule vr) => !vr.Name.Equals(ruleName, StringComparison.Ordinal)).ToList();
		}
		_rulesDictionary = _rulesDictionary.Where<KeyValuePair<Type, List<ValidationRule>>>((KeyValuePair<Type, List<ValidationRule>> r) => r.Value.Count > 0).ToDictionary((KeyValuePair<Type, List<ValidationRule>> r) => r.Key, (KeyValuePair<Type, List<ValidationRule>> r) => r.Value);
	}

	/// <summary>
	/// Removes a rule by key.
	/// </summary>
	/// <param name="key">The key of the rule to be removed.</param>
	/// <param name="rule">The rule to be removed.</param>
	/// <returns>true if the rule is successfully removed; otherwise, false.</returns>
	public bool Remove(Type key, ValidationRule rule)
	{
		if (_rulesDictionary.TryGetValue(key, out List<ValidationRule> value))
		{
			return value.Remove(rule);
		}
		return false;
	}

	/// <summary>
	/// Removes the first rule  that matches the provided rule from the list of rules.
	/// </summary>
	/// <param name="rule">The rule to be removed.</param>
	/// <returns>true if the rule is successfully removed; otherwise, false.</returns>
	public bool Remove(ValidationRule rule)
	{
		return _rulesDictionary.Values.FirstOrDefault((List<ValidationRule> x) => x.Remove(rule)) != null;
	}

	/// <summary>
	/// Clears all rules in this rule set.
	/// </summary>
	public void Clear()
	{
		_rulesDictionary.Clear();
	}

	/// <summary>
	/// Determines whether the rule set contains an element with the specified key.
	/// </summary>
	/// <param name="key">The key to locate in the rule set.</param>
	/// <returns>true if the rule set contains an element with the key; otherwise, false.</returns>
	public bool ContainsKey(Type key)
	{
		return _rulesDictionary.ContainsKey(key);
	}

	/// <summary>
	/// Determines whether the provided rule is contained in the specified key in the rule set.
	/// </summary>
	/// <param name="key">The key to locate.</param>
	/// <param name="rule">The rule to locate.</param>
	/// <returns></returns>
	public bool Contains(Type key, ValidationRule rule)
	{
		if (_rulesDictionary.TryGetValue(key, out List<ValidationRule> value))
		{
			return value.Contains(rule);
		}
		return false;
	}

	/// <summary>
	/// Gets the rules associated with the specified key.
	/// </summary>
	/// <param name="key">The key whose rules to get.</param>
	/// <param name="rules">When this method returns, the rules associated with the specified key, if the
	///  key is found; otherwise, an empty <see cref="T:System.Collections.Generic.IList`1" /> object.
	///  This parameter is passed uninitialized.</param>
	/// <returns>true if the specified key has rules.</returns>
	public bool TryGetValue(Type key, out List<ValidationRule>? rules)
	{
		return _rulesDictionary.TryGetValue(key, out rules);
	}

	/// <summary>
	/// Get the enumerator.
	/// </summary>
	/// <returns>The enumerator.</returns>
	public IEnumerator<ValidationRule> GetEnumerator()
	{
		foreach (List<ValidationRule> value in _rulesDictionary.Values)
		{
			foreach (ValidationRule item in value)
			{
				yield return item;
			}
		}
	}

	private static ValidationRuleSet BuildDefaultRuleSet()
	{
		ValidationRuleSet validationRuleSet = new ValidationRuleSet();
		Type typeFromHandle = typeof(ValidationRule);
		PropertyInfo[] validationRuleTypes = GetValidationRuleTypes();
		foreach (PropertyInfo propertyInfo in validationRuleTypes)
		{
			if (typeFromHandle.IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.GetValue(null) is ValidationRule validationRule)
			{
				validationRuleSet.Add(validationRule.ElementType, validationRule);
			}
		}
		return validationRuleSet;
	}

	internal static PropertyInfo[] GetValidationRuleTypes()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.AddRange(typeof(OpenApiComponentsRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiContactRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiDocumentRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiExtensibleRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiExternalDocsRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiInfoRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiLicenseRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiOAuthFlowRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiServerRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiResponseRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiResponsesRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiSchemaRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiTagRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiPathsRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		list.AddRange(typeof(OpenApiParameterRules).GetProperties(BindingFlags.Static | BindingFlags.Public));
		return list.ToArray();
	}
}
