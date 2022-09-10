using System;
using Islander.Attributes;

namespace Islander.Data
{
	public enum CraftworksDemandShift : uint
	{
		Skyrocketing = 0,
		Increasing = 1,
		None = 2,
		Decreasing = 3,
		Plummeting = 4,

		[Text("")]
		Invalid = 5
	}
}