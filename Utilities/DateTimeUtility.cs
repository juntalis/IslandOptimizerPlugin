using System;

namespace Islander.Utilities
{
	public static class DateTimeUtility
	{
		private const DayOfWeek WeeklyResetDay = DayOfWeek.Tuesday;
		private static readonly TimeSpan ResetTime = TimeSpan.FromHours(8);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Excepts a UTC-based starting time</param>
		/// <returns></returns>
		public static DateTime GetNextWeeklyReset(DateTime startTime)
		{
			int daysToAdd = 0;
			if(startTime.DayOfWeek > WeeklyResetDay) {
				daysToAdd = DayOfWeek.Saturday - startTime.DayOfWeek + 1;
				daysToAdd += (int)WeeklyResetDay;
			} else if(startTime.DayOfWeek < WeeklyResetDay) {
				daysToAdd = WeeklyResetDay - startTime.DayOfWeek;
			} else if(startTime.DayOfWeek == WeeklyResetDay && startTime.TimeOfDay >= ResetTime) {
				daysToAdd = 7;
			}
			
			DateTime nextResetDay = startTime.AddDays(daysToAdd);
			return new DateTime(
				nextResetDay.Year, nextResetDay.Month, nextResetDay.Day,
				ResetTime.Hours, ResetTime.Minutes, ResetTime.Seconds
			);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Excepts a UTC-based starting time</param>
		/// <returns></returns>
		public static DateTime GetNextDailyReset(DateTime startTime)
		{
			DateTime nextDay = startTime.AddDays(1);
			return new DateTime(
				nextDay.Year, nextDay.Month, nextDay.Day,
				ResetTime.Hours, ResetTime.Minutes, ResetTime.Seconds
			);
		}
	}
}
