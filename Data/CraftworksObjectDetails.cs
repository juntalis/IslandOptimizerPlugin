using System;
using System.Collections.Generic;
using Islander.Data.Sheets;
using Islander.Extensions;
using Islander.Structs;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace Islander.Data
{
	public class CraftworksObjectDetails
	{
		private readonly CraftworksObjectManager objectManager;

		internal CraftworksObjectDetails(CraftworksObject craftworkObject, CraftworksObjectManager craftworksObjectManager)
		{
			Craftwork = craftworkObject;
			objectManager = craftworksObjectManager;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="workshopRank"></param>
		/// <param name="efficiencyBonus"></param>
		/// <param name="groove"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <returns></returns>
		public uint GetTotalValue(uint workshopRank, bool efficiencyBonus, uint groove)
		{
			uint efficiencyBonusMod = efficiencyBonus ? 2U : 1U;
			double baseValue = Convert.ToDouble(BaseValue);
			double grooveModifier = (1 + Convert.ToDouble(groove)) / 100;
			double workshopRankModifier = objectManager.GetWorkshopRank(workshopRank).Modifier;
			double preTotal = PopularityModifier * SupplyModifier * Math.Floor(baseValue * workshopRankModifier * grooveModifier);
			return efficiencyBonusMod * Convert.ToUInt32(Math.Floor(preTotal));
		}

		public CraftworksObject Craftwork { get; }

		#region Internally-Set Properties

		public CraftworksSupplyType Supply { get; internal set; }

		public string SupplyText => Supply.Value.GetTextValue();

		public CraftworksDemandShift DemandShift { get; internal set; }

		public string DemandShiftText => DemandShift.GetTextValue();

		public CraftworksPopularityType Popularity { get; internal set; }

		public string PopularityText => Popularity.Value.GetTextValue();

		#endregion

		#region Delegating Methods

		#region Profit Variables

		public uint BaseValue => Craftwork.Value;

		public uint SupplyRatio => Supply.Ratio;

		public uint PopularityRatio => Popularity.Ratio;

		public double SupplyModifier => Supply.Modifier;

		public double PopularityModifier => Popularity.Modifier;

		#endregion

		public string Name => Craftwork.Name;

		public bool IsValid => Craftwork.IsValid;

		public uint RowId => Craftwork.RowId;

		public LazyRow<Item> Item => Craftwork.Item;

		public List<CraftworksObjectTheme> Themes => Craftwork.Themes;

		public List<CraftworksIngredient> Ingredients => Craftwork.Ingredients;

		public ushort ResultAmount => Craftwork.ResultAmount;

		public ushort Hours => Craftwork.Hours;

		public bool HasMatchingTheme(CraftworksObject other) => Craftwork.HasMatchingTheme(other);

		public int CountTimeGatedIngredients() => Craftwork.CountTimeGatedIngredients();

		#endregion
	}
}
