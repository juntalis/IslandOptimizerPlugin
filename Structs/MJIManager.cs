﻿using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.Attributes;

namespace Islander.Structs
{
	/// <summary>
	/// Manager struct (?) for Island Sanctuary (internally MJI).
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 0x328)]
	public unsafe partial struct MJIManager
	{
		/// <summary>
		/// Reports if the player is currently on the Island Sanctuary.
		/// </summary>
		// Not actually sure about the accuracy of this name. It's a guess based on the fact that the map system and target
		// system appear to change their behavior when this is set to 1, plus verification with how it looks in game.
		[FieldOffset(0x06)]
		public bool IsPlayerInSanctuary;

		/// <summary>
		/// The current Sanctuary Rank of the player's island.
		/// </summary>
		[FieldOffset(0x29)]
		public byte CurrentRank;

		/// <summary>
		/// The XP earned for the current Sanctuary rank.
		/// </summary>
		[FieldOffset(0x2C)]
		public uint CurrentXP;

		/// <summary>
		/// The current development level of the player's village on their island. Controls what building zones are
		/// available.
		/// </summary>
		/// <remarks>
		/// Allowed building locations are part of the MJIBuildingPlace (+0x10) and MJILandmarkPlace (+0x10) Lumina sheets.
		/// </remarks>
		[FieldOffset(0x31)]
		public byte VillageDevelopmentLevel;

		/// <summary>
		/// An array of booleans representing if a specific item is (un)locked. Locked/unavailable items are set to true,
		/// while those that are unlocked are false. This array is indexed by RowID from the MJIItemPouch table.
		/// An item appears to be unlocked upon being gathered or crafted for the first time.
		/// <seealso cref="IsPouchItemLocked" />
		/// </summary>
		[FieldOffset(0x3A)]
		public fixed byte LockedPouchItems[66];

		/// <summary>
		/// A reference to the current set of popularity scores given to craftworks on the player's island. The actual
		/// popularity scores can be pulled from the MJICraftworksPopularity sheet using this value as a Row ID.
		/// </summary>
		[FieldOffset(0x2E8)]
		public byte CurrentPopularity;

		/// <summary>
		/// A reference to the next cycle's popularity scores (called "predicted demand" in-game). Follows the same rules
		/// as <see cref="CurrentPopularity" />.
		/// </summary>
		[FieldOffset(0x2E9)]
		public byte NextPopularity;

		/// <summary>
		/// An array of bytes representing the current supply and demand shift for each craftwork that the player can
		/// create. Information for a specific item can be retrieved by querying the RowID for the item under inspection.
		/// <br /><br />
		/// The current supply value is stored in the upper half of each byte, while the current demand shift is stored in
		/// the lower half.
		/// </summary>
		[FieldOffset(0x2EA)]
		public fixed byte SupplyAndDemandShifts[62];

		/// <summary>
		/// Retrieve an instance of IslandSanctuaryManager for consumption.
		/// </summary>
		/// <returns>Returns a pointer to the game's IslandSanctuaryManager instance.</returns>
		[MemberFunction("E8 ?? ?? ?? ?? 8B 50 10", IsStatic = true)]
		public static partial MJIManager* Instance();

		/// <summary>
		/// Check if a specific MJIRecipe is *unlocked*. Does not care if the item has been crafted.
		/// </summary>
		/// <param name="recipeId">The recipe ID to check for.</param>
		/// <returns>Returns true if the recipe can be crafted, false otherwise.</returns>
		[MemberFunction("0F B7 C2 80 E2 07")]
		public partial bool IsRecipeUnlocked(ushort recipeId);

		/// <summary>
		/// Check if a specific item in the Island Pouch is (un)locked.
		/// See <see cref="LockedPouchItems" /> for more information. This method simply looks a value up from that
		/// array.
		/// </summary>
		/// <param name="itemId">The MJIItemPouch row ID to look up.</param>
		/// <returns>Returns true if the item is locked and/or hidden to the player.</returns>
		[MemberFunction("0F B7 C2 0F B6 44 08")]
		public partial bool IsPouchItemLocked(ushort itemId);

		/// <summary>
		/// Return the Supply value for a specified craftwork.
		/// </summary>
		/// <param name="itemId">The Craftwork ID to look up</param>
		/// <returns>Returns an enum value.</returns>
		public byte GetSupplyDefineIdForCraftwork(uint itemId)
		{
			return (byte)((SupplyAndDemandShifts[itemId] & 0xF0) >> 4);
		}

		/// <summary>
		/// Return the Demand Shift value for a specified craftwork.
		/// </summary>
		/// <param name="itemId">The Craftwork ID to look up</param>
		/// <returns>Returns an enum value.</returns>
		public byte GetDemandShiftForCraftwork(uint itemId)
		{
			return (byte)(SupplyAndDemandShifts[itemId] & 0x0F);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool TryGetInstance(out MJIManager* instance)
		{
			instance = MJIManager.Instance();
			return (IntPtr)instance != IntPtr.Zero;
		}
	}
}
