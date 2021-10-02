using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli
{
    /// <summary>
    /// Provides an IProgress{T} for Console applications that invokes callbacks for each reported progress value.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
    /// <remarks>
    /// This implementation does not use a <see cref="System.Threading.SynchronizationContext"/> instance.
    /// Callbacks to the handler provided to the constructor or event handlers registered with
    /// the <see cref="ProgressChanged"/> event are invoked on the currently running thread.
    /// </remarks>
    class ConsoleProgress<T> : IProgress<T>
    {
        private readonly Action<T> handler;

        /// <summary>Initializes the <see cref="ConsoleProgress{T}"/>.</summary>
        public ConsoleProgress()
        {
        }

        /// <summary>Initializes the <see cref="ConsoleProgress{T}"/> with the specified callback.</summary>
        /// <param name="handler">
        /// A handler to invoke for each reported progress value.  This handler will be invoked
        /// in addition to any delegates registered with the <see cref="ProgressChanged"/> event.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="handler"/> is null (Nothing in Visual Basic).</exception>
        public ConsoleProgress(Action<T> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.handler = handler;
        }

        /// <summary>Raised for each reported progress value.</summary>
        public event EventHandler<T> ProgressChanged;

        /// <summary>Reports a progress change and invokes the action and event callbacks.</summary>
        /// <param name="value">The value of the updated progress.</param>
        protected virtual void OnReport(T value)
        {
            handler?.Invoke(value);
            ProgressChanged?.Invoke(this, value);
        }

        /// <summary>Reports a progress change.</summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<T>.Report(T value)
        {
            OnReport(value);
        }
    }
}
