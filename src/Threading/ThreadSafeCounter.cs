// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace SystemEx.Threading;

[NonCopyable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct ThreadSafeCounter<T>(T initialValue) where T : unmanaged
#if NET7_0_OR_GREATER
  , System.Numerics.IBinaryInteger<T>
#endif
{
    static ThreadSafeCounter()
    {
        if (typeof(T) != typeof(int) && typeof(T) != typeof(uint) && typeof(T) != typeof(long) && typeof(T) != typeof(ulong))
            throw new TypeInitializationException(
                typeof(ThreadSafeCounter<T>).FullName,
                new Exception("Generic type argument must be one of [int, uint, long, ulong]"));
    }

    private T _value = initialValue;

    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            {
                int value = Volatile.Read(ref Unsafe.As<T, int>(ref _value));
                return Unsafe.As<int, T>(ref value);
            }
            else
            {
                
                long value = Volatile.Read(ref Unsafe.As<T, long>(ref _value));
                return Unsafe.As<long, T>(ref value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            {
                Interlocked.Exchange(ref Unsafe.As<T, int>(ref _value), Unsafe.As<T, int>(ref value));
            }
            else
            {
                Interlocked.Exchange(ref Unsafe.As<T, long>(ref _value), Unsafe.As<T, long>(ref value));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Reset()
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            int previous = Interlocked.Exchange(ref Unsafe.As<T, int>(ref _value), 0);
            return Unsafe.As<int, T>(ref previous);
        }
        else
        {
            long previous = Interlocked.Exchange(ref Unsafe.As<T, long>(ref _value), 0L);
            return Unsafe.As<long, T>(ref previous);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Increment()
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            int value = Interlocked.Add(ref Unsafe.As<T, int>(ref _value), 1);
            return Unsafe.As<int, T>(ref value);
        }
        else
        {
            long value = Interlocked.Add(ref Unsafe.As<T, long>(ref _value), 1L);
            return Unsafe.As<long, T>(ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Decrement()
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            int value = Interlocked.Add(ref Unsafe.As<T, int>(ref _value), -1);
            return Unsafe.As<int, T>(ref value);
        }
        else
        {
            long value = Interlocked.Add(ref Unsafe.As<T, long>(ref _value), -1L);
            return Unsafe.As<long, T>(ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Add(T amount)
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            int value = Interlocked.Add(ref Unsafe.As<T, int>(ref _value), Unsafe.As<T, int>(ref amount));
            return Unsafe.As<int, T>(ref value);
        }
        else
        {
            long value = Interlocked.Add(ref Unsafe.As<T, long>(ref _value), Unsafe.As<T, long>(ref amount));
            return Unsafe.As<long, T>(ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CompareExchange(T value, T comparand)
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            int original = Interlocked.CompareExchange(
                ref Unsafe.As<T, int>(ref _value),
                Unsafe.As<T, int>(ref value),
                Unsafe.As<T, int>(ref comparand));

            return Unsafe.As<int, T>(ref original);
        }
        else
        {
            long original = Interlocked.CompareExchange(
                ref Unsafe.As<T, long>(ref _value),
                Unsafe.As<T, long>(ref value),
                Unsafe.As<T, long>(ref comparand));

            return Unsafe.As<long, T>(ref original);
        }
    }
}