using System;
using Islander.Attributes;

namespace Islander.Data
{
	public enum CraftworksSupply : uint
	{
		Nonexistent = 0,
		Insufficient = 1,
		Sufficient = 2,
		Surplus = 3,
		Overflowing = 4,
		[Text("")]
		Invalid = 5
	}
}