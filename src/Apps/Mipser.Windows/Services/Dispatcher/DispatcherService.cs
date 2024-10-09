// Adam Dernis 2024

using Mipser.Services.Dispatcher;
using System;
using Windows.System;

namespace Mipser.Windows.Services.Dispatcher;

/// <summary>
/// A <see cref="IDispatcherService"/> implementation for the windows client.
/// </summary>
public class DispatcherService : IDispatcherService
{
    private DispatcherQueue? _dispatcherQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherService"/> class.
    /// </summary>
    public DispatcherService()
    {
    }

    internal void Init()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    /// <inheritdoc/>
    public void RunOnUIThread(Action action)
    {
        _dispatcherQueue?.TryEnqueue(DispatcherQueuePriority.Normal, action.Invoke);
    }
}
