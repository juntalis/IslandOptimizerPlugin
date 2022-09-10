using System;
using Islander.Attributes;

namespace Islander.Data
{
	public enum CraftworksPopularity : uint
	{
	
		[Text("")]
		None = 0,
		[Text("Very High")]
		VeryHigh = 1,
		High = 2,
		Average = 3,
		Low = 4,
		[Text("")]
		Invalid = 5
	}
}

