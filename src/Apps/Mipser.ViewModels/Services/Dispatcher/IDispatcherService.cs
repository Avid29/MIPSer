// Adam Dernis 2024

using System;

namespace Mipser.Services.Dispatcher;

/// <summary>
/// An interface for a dispatcher service.
/// </summary>
public interface IDispatcherService
{
    /// <summary>
    /// Runs an <see cref="Action"/> on the UI Thread.
    /// </summary>
    void RunOnUIThread(Action action);
}
