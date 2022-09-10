#nullable enable
using System;
using Dalamud.Utility;
using Islander.Attributes;

namespace Islander.Extensions
{
	/// <summary>
	/// Class extensions to <see cref="Enum"/>
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets an <see cref="TextAttribute"/> attribute on an enum.
		/// </summary>
		/// <param name="self">The enum value that has an attached attribute.</param>
		/// <returns>The attached attribute, if any.</returns>
		public static string GetTextValue(this Enum self)
		{
			TextAttribute? textAttribute = self.GetAttribute<TextAttribute>();
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if(textAttribute == null) {
				return Enum.GetName(self.GetType(), (object)self) ?? String.Empty;
			}
			return textAttribute.Value;
		}
	}
}

