using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Class containing validation rule logic.
/// </summary>
public abstract class ValidationRule
{
	/// <summary>
	/// Element Type.
	/// </summary>
	internal abstract Type ElementType { get; }

	/// <summary>
	/// Validation rule Name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Validate the object.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="item">The object item.</param>
	internal abstract void Evaluate(IValidationContext context, object item);

	internal ValidationRule(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		Name = name;
	}
}
/// <summary>
/// Class containing validation rule logic for <see cref="T:Microsoft.OpenApi.IOpenApiElement" />.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ValidationRule<T> : ValidationRule
{
	private readonly Action<IValidationContext, T> _validate;

	internal override Type ElementType => typeof(T);

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.ValidationRule" /> class.
	/// </summary>
	/// <param name="name">Validation rule name.</param>
	/// <param name="validate">Action to perform the validation.</param>
	public ValidationRule(string name, Action<IValidationContext, T> validate)
		: base(name)
	{
		_validate = Utils.CheckArgumentNull(validate, "validate");
	}

	internal override void Evaluate(IValidationContext context, object item)
	{
		if (item != null)
		{
			if (!(item is T arg))
			{
				throw new ArgumentException(string.Format(SRResource.InputItemShouldBeType, typeof(T).FullName));
			}
			_validate(context, arg);
		}
	}
}
