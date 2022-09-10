#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Data;
using Islander.Data.Reset;
using Islander.Data.Sheets;
using Islander.Extensions;
using Islander.Structs;
using Islander.Utilities;
using Lumina.Excel;

namespace Islander.Data
{
	public class CraftworksObjectManager : IDisposable
	{
		// private readonly DataManager data;
		// private readonly MJIManager manager;
		private readonly ConcurrentDictionary<UInt32, CraftworksObjectDetails> objectsDetails = new ConcurrentDictionary<uint, CraftworksObjectDetails>();
		private readonly Lazy<CraftworksSupplyType[]> lazySupplyTypes;
		private readonly Lazy<CraftworksWorkshopRank[]> lazyWorkshopRanks;
		private readonly Lazy<CraftworksPopularityType[]> lazyPopularityTypes;
		private readonly Resetting<CraftworksObjectPopularity> weeklyPopularity;

		public unsafe CraftworksObjectManager(MJIManager* mgiManager, DataManager dataManager)
		{
			Data = dataManager;
			Manager = mgiManager;
			lazySupplyTypes = new Lazy<CraftworksSupplyType[]>(GetSupplyTypes);
			lazyWorkshopRanks = new Lazy<CraftworksWorkshopRank[]>(GetWorkshopRanks);
			lazyPopularityTypes = new Lazy<CraftworksPopularityType[]>(GetPopularityTypes);
			weeklyPopularity = new Resetting<CraftworksObjectPopularity>(GetCurrentPopularity, ResetBehavior.ResetsWeekly);
		}

		public DataManager Data { get; private set; }

		public unsafe MJIManager* Manager { get; private set; }

		public unsafe IntPtr ManagerPtr => (IntPtr)Manager;

		public unsafe byte CurrentPopularityId => Manager->CurrentPopularity;

		public unsafe IntPtr SupplyDemandPtr => (IntPtr)Manager->SupplyAndDemandShifts;

		public byte[] SupplyAndDemandShiftBytes => GetSupplyDemandBytes();

		public CraftworksWorkshopRank[] WorkshopRanks => lazyWorkshopRanks.Value;

		public CraftworksObjectPopularity CurrentPopularity => weeklyPopularity.Value!;

		public CraftworksPopularityType[] PopularityTypes => lazyPopularityTypes.Value;

		public CraftworksSupplyType[] SupplyTypes => lazySupplyTypes.Value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workshopRank"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public CraftworksWorkshopRank GetWorkshopRank(uint workshopRank)
		{
			if(workshopRank >= (uint)WorkshopRanks.Length) {
				throw new ArgumentOutOfRangeException(nameof(workshopRank), $"Invalid workshop rank specified: {workshopRank} >= {WorkshopRanks.Length}!");
			}
			return WorkshopRanks[workshopRank];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public CraftworksSupplyType GetObjectSupply(uint objectId)
		{
			uint supplyDefineId = GetSupplyDefineIdForCraftwork(objectId);
			if(supplyDefineId >= (uint)SupplyTypes.Length) {
				throw new IndexOutOfRangeException($"Invalid SupplyDefine id returned from MJIManager: {supplyDefineId}!");
			}
			return SupplyTypes[supplyDefineId];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public CraftworksPopularityType GetObjectPopularity(uint objectId)
		{
			uint popularityTypeId = CurrentPopularity.GetCraftworkPopularityTypeId(objectId);
			if(popularityTypeId >= (uint)PopularityTypes.Length) {
				throw new IndexOutOfRangeException($"Invalid PopularityType id returned from Popularity.GetCraftworkPopularityTypeId({objectId}): {popularityTypeId}!");
			}
			return PopularityTypes[popularityTypeId];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public CraftworksDemandShift GetObjectDemandShift(uint objectId)
		{
			uint demandShiftValue = GetDemandShiftForCraftwork(objectId);
			if(demandShiftValue >= (uint)CraftworksDemandShift.Invalid) {
				demandShiftValue = (uint)CraftworksDemandShift.Invalid;
			}
			return (CraftworksDemandShift)demandShiftValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="craftwork"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public CraftworksObjectDetails? GetObjectDetails(CraftworksObject craftwork)
		{
			if(!objectsDetails.TryGetValue(craftwork.RowId, out CraftworksObjectDetails? result)) {
				if(!craftwork.IsValid) {
					result = new CraftworksObjectDetails(craftwork, this);
					objectsDetails[craftwork.RowId] = result;
					return result;
				}
				result = new CraftworksObjectDetails(craftwork, this) {
					Supply = GetObjectSupply(craftwork.RowId),
					Popularity = GetObjectPopularity(craftwork.RowId),
					DemandShift = GetObjectDemandShift(craftwork.RowId),
				};
				objectsDetails[craftwork.RowId] = result;
			}
			return result;
		}

		#region Manager Wrappers

		private unsafe uint GetCurrentPopularityId() => Convert.ToUInt32(Manager->CurrentPopularity);

		private unsafe uint GetDemandShiftForCraftwork(uint rowId) => Convert.ToUInt32(Manager->GetDemandShiftForCraftwork(rowId));

		private unsafe uint GetSupplyDefineIdForCraftwork(uint rowId) => Convert.ToUInt32(Manager->GetSupplyDefineIdForCraftwork(rowId));

		private byte[] GetSupplyDemandBytes() => Dalamud.Memory.MemoryHelper.ReadRaw(SupplyDemandPtr, 62);

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CraftworksObjectDetails?> GetValidObjectsDetails()
		{
			return GetSheet<CraftworksObject>().GetValidRows().Select(GetObjectDetails);
		}
		
		#region Lazy Loaded Sheets/Rows

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private CraftworksWorkshopRank[] GetWorkshopRanks() => GetSheetRows<CraftworksWorkshopRank>().ToArray();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private CraftworksSupplyType[] GetSupplyTypes() => GetSheetRows<CraftworksSupplyType>().ToArray();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private CraftworksPopularityType[] GetPopularityTypes() => GetSheetRows<CraftworksPopularityType>().ToArray();

		/// <summary>
		/// Loads the current popularity details.
		/// </summary>
		/// <returns></returns>
		private CraftworksObjectPopularity GetCurrentPopularity()
		{
			uint popularityId = GetCurrentPopularityId();
			return GetSheetRowById<CraftworksObjectPopularity>(popularityId);
		}

		#endregion

		/// <inheritdoc cref="GetSheetRowById{TRow}(uint)"/>
		private TRow GetSheetRowById<TRow>(ushort rowId) where TRow : ExcelRow => GetSheetRowById<TRow>((uint)rowId);

		/// <inheritdoc cref="GetSheetRowById{TRow}(uint)"/>
		private TRow GetSheetRowById<TRow>(byte rowId) where TRow : ExcelRow => GetSheetRowById<TRow>((uint)rowId);

		/// <summary>
		/// Row accessor with null checking. Throws a <see cref="KeyNotFoundException"/> on failure.
		/// </summary>
		/// <typeparam name="TRow"></typeparam>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		private TRow GetSheetRowById<TRow>(uint rowId)
			where TRow : ExcelRow
		{
			TRow? row = GetSheet<TRow>().GetRow(rowId);
			if(row == null) {
				throw new KeyNotFoundException($"Could not locate row of type {typeof(TRow).FullName} by id #{rowId}");
			}
			return row;
		}

		/// <summary>
		/// Rows accessor with null checking. Throws a <see cref="KeyNotFoundException"/> on failure.
		/// </summary>
		/// <typeparam name="TRow"></typeparam>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		private IEnumerable<TRow> GetSheetRows<TRow>(Func<TRow, bool>? predicate = null)
			where TRow : ExcelRow
		{
			return GetSheet<TRow>().Where(predicate ?? new Func<TRow, bool>(_ => true));
		}

		/// <summary>
		/// Sheet accessor with null checking. Throws a <see cref="KeyNotFoundException"/> on failure.
		/// </summary>
		/// <typeparam name="TRow"></typeparam>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		private ExcelSheet<TRow> GetSheet<TRow>()
			where TRow : ExcelRow
		{
			ExcelSheet<TRow>? sheet = Data.GetExcelSheet<TRow>();
			if(sheet == null) {
				Logger.LogError($"Failed to load ExcelSheet<{typeof(TRow).FullName}>!");
				throw new KeyNotFoundException($"Failed to load excel sheet for {typeof(TRow).FullName}");
			}
			return sheet;
		}

		#region IDisposable

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			weeklyPopularity.Dispose();
		#pragma warning disable CS8625
			Data = null;
		#pragma warning restore CS8625
		}

		#endregion
	}
}
