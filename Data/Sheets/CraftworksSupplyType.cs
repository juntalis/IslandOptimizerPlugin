#nullable enable
using System;
using Islander.Utilities;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksSupplyDefine", 0xEAB7D419U)]
	public class CraftworksSupplyType : RatioSheet, IEquatable<CraftworksSupplyType>
	{
		public short Supply { get; set; }

		public bool IsValid => Value != CraftworksSupply.Invalid;

		public CraftworksSupply Value => RowId >= (uint)CraftworksSupply.Invalid ?
		                                CraftworksSupply.Invalid :
		                                (CraftworksSupply)RowId;

		#region Overrides of BaseRatioSheet

		public override void PopulateRatio(RowParser parser, GameData gameData, Language language)
		{
			Ratio = parser.ReadColumn<ushort>(1);
		}

		#endregion

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);
			Supply = parser.ReadColumn<short>(0);
		}

		#region Implementation of IEquatable<CraftworksSupplyType>

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equals(CraftworksSupplyType? other)
		{
			return other != null && RowId == other.RowId;
		}

		#endregion
	}
}
