#nullable enable
using System;
using System.Threading;
using Islander.Utilities;

namespace Islander.Data.Reset
{
	public class Resetting<TValue> : IDisposable
		where TValue : class
	{
		private TValue? value = null;
		private ReaderWriterLockSlim rwLock;
		private readonly Func<TValue?> valueGetter;

		public Resetting(Func<TValue> getter, ResetBehavior behavior, int milliSecondsTimeout = 10000)
		{
			Behavior = behavior;
			valueGetter = getter;
			NextResetTime = DateTime.MinValue;
			Timeout = milliSecondsTimeout;
			rwLock = new ReaderWriterLockSlim();
		}

		public int Timeout { get; set; }

		public ResetBehavior Behavior { get; }

		public DateTime NextResetTime { get; private set; }

		public TValue? Value => GetValue();

		public TValue? GetValue()
		{
			TValue? result = null;
			if(rwLock.TryEnterReadLock(Timeout)) {
				bool needsReset = false;
				try {
					result = value;
					needsReset = ShouldResetValue();
				} finally {
					rwLock.ExitReadLock();
				}
				if(needsReset) {
					result = ResetValue();
				}
			} else {
				Logger.LogWarning($"Timeout while entering read lock in Resetting<{typeof(TValue)}>::GetValue()");
			}
			return result;
		}

		/// <summary>
		/// Executing within the scope of a read lock
		/// </summary>
		/// <returns></returns>
		private bool ShouldResetValue()
		{
			return value == null || NextResetTime == DateTime.MinValue || DateTime.UtcNow >= NextResetTime;
		}

		private TValue? ResetValue()
		{
			TValue? result = null;
			if(rwLock.TryEnterWriteLock(Timeout)) {
				try {
					DateTime nextResetTime = GetNextResetTime();
					result = valueGetter();
					if(result != null) {
						value = result;
						NextResetTime = nextResetTime;
					}
				} finally {
					rwLock.ExitWriteLock();
				}
			} else {
				Logger.LogWarning($"Timeout while entering write lock in Resetting<{typeof(TValue)}>::ResetValue()");
			}
			return result;
		}
		
		private DateTime GetNextResetTime()
		{
			DateTime now = DateTime.UtcNow;
			if(Behavior == ResetBehavior.ResetsDaily) {
				return DateTimeUtility.GetNextDailyReset(now);
			}
			return DateTimeUtility.GetNextWeeklyReset(now);
		}

		#region IDisposable

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			value = null;
			rwLock.Dispose();
		}

		#endregion
	}
}
