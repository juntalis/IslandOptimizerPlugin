#nullable enable
using System;
using Dalamud.Game.Text.SeStringHandling;
using Islander.Utilities;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksObjectTheme", 0xb40b0011)]
	public class CraftworksObjectTheme : ExcelRow, IEquatable<CraftworksObjectTheme>
	{
		public SeString? Name { get; set; }

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);
			Logger.Log($"{GetDefaultColumnValue()}");
			Name = parser.ReadColumn<SeString>(0);
		}

		#region Implementation of IEquatable<MJICraftworksObjectTheme>

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equals(CraftworksObjectTheme? other)
		{
			return other != null && RowId == other.RowId;
		}

		#endregion
	}
}
