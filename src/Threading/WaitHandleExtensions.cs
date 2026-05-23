// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SystemEx.Threading;

public static class WaitHandleExtensions
{
    private static readonly WaitOrTimerCallback SetTcsResultCallback =
        static (state, timedOut) => ((TaskCompletionSource<bool>)state!).TrySetResult(!timedOut);

    private static readonly Action<object?> SetTcsCanceledCallback =
        static state => ((TaskCompletionSource<bool>)state!).TrySetCanceled();

    public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, CancellationToken ct = default) =>
        WaitOneAsync(waitHandle, Timeout.InfiniteTimeSpan, ct);

    public static async Task<bool> WaitOneAsync(this WaitHandle waitHandle, TimeSpan timeout, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        CancellationTokenRegistration ctr = default;
        TaskCompletionSource<bool> tcs = new();

        RegisteredWaitHandle handle = ThreadPool.RegisterWaitForSingleObject(
            waitHandle, SetTcsResultCallback, tcs, timeout, executeOnlyOnce: true);

        try
        {
            if (ct.CanBeCanceled)
#if NET5_0_OR_GREATER
                ctr = ct.UnsafeRegister(SetTcsCanceledCallback, tcs);
#else
                ctr = ct.Register(SetTcsCanceledCallback, tcs, useSynchronizationContext: false);
#endif
            return await tcs.Task;
        }
        finally
        {
            ctr.Dispose();
            handle.Unregister(null);
        }
    }
}