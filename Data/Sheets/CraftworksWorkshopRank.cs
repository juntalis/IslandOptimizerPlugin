#nullable enable
using System;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksRankRatio", 0xAED1D46CU)]
	public class CraftworksWorkshopRank : RatioSheet, IEquatable<CraftworksWorkshopRank>
	{
		#region Implementation of IEquatable<CraftworksWorkshopRank>

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equals(CraftworksWorkshopRank? other)
		{
			return other != null && RowId == other.RowId;
		}

		#endregion
	}
}
