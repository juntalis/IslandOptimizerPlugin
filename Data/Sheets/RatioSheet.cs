using System;
using Islander.Utilities;
using Lumina.Data;
using Lumina;
using Lumina.Excel;

namespace Islander.Data.Sheets
{
	public abstract class RatioSheet : ExcelRow
	{
		public ushort Ratio { get; set; }

		public double Modifier => Convert.ToDouble(Ratio) / 100;

		public virtual void PopulateRatio(RowParser parser, GameData gameData, Language language)
		{
			Ratio = parser.ReadColumn<ushort>(0);
		}

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			Logger.Log($"{GetDefaultColumnValue()}");
			base.PopulateData(parser, gameData, language);
			PopulateRatio(parser, gameData, language);
		}
	}
}
