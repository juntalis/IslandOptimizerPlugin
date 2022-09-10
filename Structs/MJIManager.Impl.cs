using System;
using System.Collections.Generic;
using Islander.Utilities;

namespace Islander.Structs
{
	public unsafe partial struct MJIManager
	{
		private static bool initialized = false;
		private static delegate* unmanaged[Stdcall]<MJIManager*> fpInstance { set; get; }

		public static partial MJIManager* Instance()
		{
			Initialize();
			if(fpInstance is null) {
				throw new InvalidOperationException("Function pointer for MJIManager::Instance is null. Did you forget to call Resolver.Initialize?");
			}

			return fpInstance();
		}
		
		private static delegate* unmanaged[Stdcall]<MJIManager*, ushort, byte> fpIsRecipeUnlocked { set; get; }

		public partial bool IsRecipeUnlocked(ushort recipeId)
		{
			if(fpIsRecipeUnlocked is null) {
				throw new InvalidOperationException("Function pointer for MJIManager::IsRecipeUnlocked is null. Did you forget to call Resolver.Initialize?");
			}

			fixed(MJIManager* thisPtr = &this) {
				return fpIsRecipeUnlocked(thisPtr, recipeId) != 0;
			}
		}

		private static delegate* unmanaged[Stdcall]<MJIManager*, ushort, byte> fpIsPouchItemLocked { set; get; }

		public partial bool IsPouchItemLocked(ushort itemId)
		{
			if(fpIsPouchItemLocked is null) {
				throw new InvalidOperationException("Function pointer for MJIManager::IsPouchItemLocked is null. Did you forget to call Resolver.Initialize?");
			}

			fixed(MJIManager* thisPtr = &this) {
				return fpIsPouchItemLocked(thisPtr, itemId) != 0;
			}
		}
		
		public static void Initialize()
		{
			if(!initialized) {
				initialized = true;
				try {
					var addressMJIManagerInstance = Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 8B 50 10");
					fpInstance = (delegate* unmanaged[Stdcall]<MJIManager*>)addressMJIManagerInstance;
				} catch(KeyNotFoundException) {
					Logger.LogWarning("function MJIManager::Instance failed to match signature E8 ?? ?? ?? ?? 8B 50 10 and is unavailable");
				}
				try {
					var addressMJIManagerIsRecipeUnlocked = Service.SigScanner.ScanText("0F B7 C2 80 E2 07");
					fpIsRecipeUnlocked = (delegate* unmanaged[Stdcall]<MJIManager*, ushort, byte>)addressMJIManagerIsRecipeUnlocked;
				} catch(KeyNotFoundException) {
					Logger.LogWarning("function MJIManager::IsRecipeUnlocked failed to match signature 0F B7 C2 80 E2 07 and is unavailable");
				}
				try {
					var addressMJIManagerIsPouchItemLocked = Service.SigScanner.ScanText("0F B7 C2 0F B6 44 08");
					MJIManager.fpIsPouchItemLocked = (delegate* unmanaged[Stdcall]<MJIManager*, ushort, byte>)addressMJIManagerIsPouchItemLocked;
				} catch(KeyNotFoundException) {
					Logger.LogWarning("function MJIManager::IsPouchItemLocked failed to match signature 0F B7 C2 0F B6 44 08 and is unavailable");
				}
			}
		}
	}
}
