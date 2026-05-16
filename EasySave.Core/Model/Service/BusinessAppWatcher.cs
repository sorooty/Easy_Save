namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Polls for a business software process and notifies registered callbacks
    /// when the software starts or stops. Callbacks receive <c>true</c> when
    /// the software is detected and <c>false</c> when it is no longer running.
    /// Transitions are edge-triggered: the callback fires once per change, not
    /// on every poll tick.
    /// </summary>
    public class BusinessAppWatcher
    {
        /// <summary>Returns true if the business software is currently running.</summary>
        public Func<bool>? IsRunning { get; set; }

        private readonly List<Action<bool>> _callbacks = new();
        private readonly object _lock = new();
        private bool _previousState;

        /// <summary>
        /// Registers a callback. If the business software is already running at
        /// registration time the callback is invoked immediately with <c>true</c>.
        /// </summary>
        public void Register(Action<bool> callback)
        {
            lock (_lock)
            {
                _callbacks.Add(callback);
                if (_previousState)
                    callback(true);
            }
        }

        /// <summary>Removes a previously registered callback.</summary>
        public void Unregister(Action<bool> callback)
        {
            lock (_lock)
            {
                _callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// Starts the polling loop. Runs until <paramref name="ct"/> is cancelled.
        /// Poll interval is 500 ms. Call this on a background thread or with
        /// <c>Task.Run</c>.
        /// </summary>
        public async Task StartAsync(CancellationToken ct)
        {
            // Establish initial state without firing callbacks
            _previousState = IsRunning?.Invoke() ?? false;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                bool current = IsRunning?.Invoke() ?? false;
                if (current == _previousState)
                    continue;

                _previousState = current;

                Action<bool>[] snapshot;
                lock (_lock)
                {
                    snapshot = _callbacks.ToArray();
                }

                foreach (var cb in snapshot)
                {
                    try { cb(current); }
                    catch { /* callbacks must not crash the watcher */ }
                }
            }
        }
    }
}
