﻿using System;
using System.Linq;
using System.Reactive.Linq;

namespace Kprocess.TabTip
{
    static class PoolingTimer
    {
        static bool Pooling;

        internal static void PoolUntilTrue(Func<bool> PoolingFunc, Action Callback, TimeSpan dueTime, TimeSpan period)
        {
            if (Pooling)
                return;

            Pooling = true;

            Observable.Timer(dueTime, period)
                .Select(_ => PoolingFunc())
                .TakeWhile(stop => stop != true)
                .Where(stop => stop == true)
                .Finally(() => Pooling = false)
                .Subscribe(_ => { }, Callback);
        }
    }
}