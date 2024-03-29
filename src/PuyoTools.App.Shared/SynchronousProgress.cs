﻿/*
 * This implementation is derived from the reference source for Progress{T}.
 * https://referencesource.microsoft.com/#mscorlib/system/progress.cs
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuyoTools.App
{
    /// <summary>
    /// Provides an IProgress{T} that invokes synchronous callbacks for each reported progress value.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
    /// <remarks>
    /// Any handler provided to the constructor or event handlers registered with
    /// the <see cref="ProgressChanged"/> event are invoked through a 
    /// <see cref="System.Threading.SynchronizationContext"/> instance captured
    /// when the instance is constructed.  If there is no current SynchronizationContext
    /// at the time of construction, the callbacks will be invoked on the currently running thread.
    /// </remarks>
    public class SynchronousProgress<T> : IProgress<T>
    {
        /// <summary>The synchronization context captured upon construction.  This will never be null.</summary>
        private readonly SynchronizationContext m_synchronizationContext;
        /// <summary>The handler specified to the constructor.  This may be null.</summary>
        private readonly Action<T> m_handler;
        /// <summary>A cached delegate used to post invocation to the synchronization context.</summary>
        private readonly SendOrPostCallback m_invokeHandlers;

        /// <summary>Initializes the <see cref="SynchronousProgress{T}"/>.</summary>
        public SynchronousProgress()
        {
            // Capture the current synchronization context.  "current" is determined by Current,
            // which doesn't consider the sync ctx flown with an ExecutionContext, avoiding
            // sync ctx reference identity issues where the sync ctx for one thread could be Current on another.
            // If there is no current context, we use a default instance targeting the currently running thread.
            m_synchronizationContext = SynchronizationContext.Current ?? SynchronousProgressStatics.DefaultContext;
            Contract.Assert(m_synchronizationContext != null);
            m_invokeHandlers = new SendOrPostCallback(InvokeHandlers);
        }

        /// <summary>Initializes the <see cref="SynchronousProgress{T}"/> with the specified callback.</summary>
        /// <param name="handler">
        /// A handler to invoke for each reported progress value.  This handler will be invoked
        /// in addition to any delegates registered with the <see cref="ProgressChanged"/> event.
        /// Depending on the <see cref="System.Threading.SynchronizationContext"/> instance captured by 
        /// the <see cref="Progress"/> at construction, it's possible that this handler instance
        /// could be invoked concurrently with itself.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="handler"/> is null (Nothing in Visual Basic).</exception>
        public SynchronousProgress(Action<T> handler) : this()
        {
            if (handler == null) throw new ArgumentNullException("handler");
            m_handler = handler;
        }

        /// <summary>Raised for each reported progress value.</summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> captured when the instance was constructed.
        /// </remarks>
        public event EventHandler<T> ProgressChanged;

        /// <summary>Reports a progress change.</summary>
        /// <param name="value">The value of the updated progress.</param>
        protected virtual void OnReport(T value)
        {
            // If there's no handler, don't bother going through the sync context.
            // Inside the callback, we'll need to check again, in case 
            // an event handler is removed between now and then.
            Action<T> handler = m_handler;
            EventHandler<T> changedEvent = ProgressChanged;
            if (handler != null || changedEvent != null)
            {
                // Post the processing to the sync context.
                // (If T is a value type, it will get boxed here.)
                m_synchronizationContext.Send(m_invokeHandlers, value);
            }
        }

        /// <summary>Reports a progress change.</summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<T>.Report(T value) { OnReport(value); }

        /// <summary>Invokes the action and event callbacks.</summary>
        /// <param name="state">The progress value.</param>
        private void InvokeHandlers(object state)
        {
            T value = (T)state;

            Action<T> handler = m_handler;
            EventHandler<T> changedEvent = ProgressChanged;

            if (handler != null) handler(value);
            if (changedEvent != null) changedEvent(this, value);
        }
    }

    /// <summary>Holds static values for <see cref="SynchronousProgress{T}"/>.</summary>
    /// <remarks>This avoids one static instance per type T.</remarks>
    internal static class SynchronousProgressStatics
    {
        /// <summary>A default synchronization context that targets the ThreadPool.</summary>
        internal static readonly SynchronizationContext DefaultContext = new SynchronizationContext();
    }
}
