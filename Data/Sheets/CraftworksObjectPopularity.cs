using System;
using Islander.Utilities;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksPopularity", 0x9441F969U)]
	public class CraftworksObjectPopularity : ExcelRow
	{
		public byte[] PopularityTypeIds { get; set; }

		public uint GetCraftworkPopularityTypeId(CraftworksObject craftwork)
		{
			return GetCraftworkPopularityTypeId(craftwork.RowId);
		}

		public uint GetCraftworkPopularityTypeId(uint objectId)
		{
			if(objectId == 0 || objectId >= PopularityTypeIds.Length) {
				throw new IndexOutOfRangeException("Invalid objectId specified for GetCraftworkPopularity!");
			}
			return (uint)PopularityTypeIds[objectId];
		}

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);
			Logger.Log($"{GetDefaultColumnValue()}");
			PopularityTypeIds = parser.ReadBytes(0, 62);
			Logger.Log(PopularityTypeIds);
		}
	}
}
