#nullable enable
using System;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJIItemCategory", 0x7a08c988)]
	public class PouchItemCategory : ExcelRow, IEquatable<PouchItemCategory>
	{
		private static readonly HashSet<PouchCategory> TimeGatedCategories = new HashSet<PouchCategory>() {
			PouchCategory.RareMaterial,
			PouchCategory.Produce,
			PouchCategory.Leavings
		};

		public SeString? Singular { get; set; }
		
		public SeString? Plural { get; set; }

		public PouchCategory Value { get; set; }

		public bool IsValid => Value is > PouchCategory.None and < PouchCategory.Invalid;

		public bool IsTimeGated => TimeGatedCategories.Contains(Value);

		#region Overrides of ExcelRow

		public override object GetDefaultColumnValue()
		{
			return Enum.GetName(Value) ?? base.GetDefaultColumnValue();
		}

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);
			Singular = parser.ReadColumn<SeString>(0);
			Plural = parser.ReadColumn<SeString>(1);
			if(RowId > (uint)PouchCategory.Handicraft) {
				Value = PouchCategory.Invalid;
			} else {
				Value = (PouchCategory)RowId;
			}
		}

		#endregion

		#region Implementation of IEquatable<PouchItemCategory>

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equals(PouchItemCategory? other)
		{
			return other != null && RowId == other.RowId;
		}

		#endregion
	}
}
