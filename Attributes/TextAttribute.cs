using System;

namespace Islander.Attributes
{
	/// <summary>
	/// Custom attribute class
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class TextAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Positional string argument</param>
		public TextAttribute(string value)
		{
			Value = value;
		}

		/// <summary>
		/// Property specified in the constructor.
		/// </summary>
		public string Value { get; }

	}
}
	
