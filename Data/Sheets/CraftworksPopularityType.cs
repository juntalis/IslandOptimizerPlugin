#nullable enable
using System;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksPopularityType", 0xAED1D46CU)]
	public class CraftworksPopularityType : RatioSheet, IEquatable<CraftworksPopularityType>
	{
		public CraftworksPopularity Value => RowId >= (uint)CraftworksPopularity.Invalid ?
		                                     CraftworksPopularity.Invalid : (CraftworksPopularity)RowId;

		public bool IsValid => Value is > CraftworksPopularity.None and < CraftworksPopularity.Invalid;
		
		#region Implementation of IEquatable<CraftworksPopularityType>

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equals(CraftworksPopularityType? other)
		{
			return other != null && RowId == other.RowId;
		}

		#endregion
	}
}
