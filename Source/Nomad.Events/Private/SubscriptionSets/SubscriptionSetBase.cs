using System;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.Events.Private {
	/*
	===================================================================================

	SubscriptionSetBase

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal abstract class SubscriptionSetBase<TArgs, TCallback> : IDisposable
		where TArgs : struct
		where TCallback : class
	{
		protected readonly ILoggerService Logger;
		protected readonly IGameEvent<TArgs> EventData;

		// Optional: used by LockFreeSubscriptionSet
		protected readonly int OwnerThreadId;

		/*
		===============
		SubscriptionSetBase
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="logger"></param>
		protected SubscriptionSetBase( IGameEvent<TArgs> eventData, ILoggerService logger ) {
			EventData = eventData;
			Logger = logger;
			OwnerThreadId = Environment.CurrentManagedThreadId;
		}

		// -----------------------------
		// Abstract storage operations
		// -----------------------------

		protected abstract void AddInternal( object subscriber, TCallback callback );
		protected abstract void RemoveInternal( object subscriber, TCallback callback );
		protected abstract void RemoveAllInternal( object subscriber );

		protected abstract void PumpInternal( in TArgs args );
		protected abstract void PumpAsyncInternal( TArgs args, CancellationToken ct );

		protected abstract void CleanupInternal();
		protected abstract void CleanupIncrementalInternal( int maxSteps );

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public virtual void Dispose() {
			CleanupInternal();
		}

		/*
		===============
		AddSubscription
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		public void AddSubscription( object subscriber, TCallback callback ) {
			ExceptionCompat.ThrowIfNull( subscriber );
			ExceptionCompat.ThrowIfNull( callback );

			AddInternal( subscriber, callback );
		}

		/*
		===============
		RemoveSubscription
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		public void RemoveSubscription( object subscriber, TCallback callback ) {
			ExceptionCompat.ThrowIfNull( subscriber );
			ExceptionCompat.ThrowIfNull( callback );

			RemoveInternal( subscriber, callback );
		}

		/*
		===============
		RemoveAllForSubscriber
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="subscriber"></param>
		public void RemoveAllForSubscriber( object subscriber ) {
			ExceptionCompat.ThrowIfNull( subscriber );
			RemoveAllInternal( subscriber );
		}

		/*
		===============
		Pump
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		public void Pump( in TArgs args ) {
			PumpInternal( in args );
		}

		/*
		===============
		PumpAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task PumpAsync( TArgs args, CancellationToken ct ) {
			PumpAsyncInternal( args, ct );
		}

		/*
		===============
		Cleanup
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Cleanup() {
			CleanupInternal();
		}

		/*
		===============
		CleanupIncremental
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="maxSteps"></param>
		public void CleanupIncremental( int maxSteps ) {
			CleanupIncrementalInternal( maxSteps );
		}

		/*
		===============
		EnsureThread
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		protected void EnsureThread() {
#if DEBUG
			if ( Environment.CurrentManagedThreadId != OwnerThreadId ) {
				throw new InvalidOperationException(
					$"SubscriptionSet '{EventData.DebugName}' is single-threaded and cannot be used from another thread." );
			}
#endif
		}
	}
}
