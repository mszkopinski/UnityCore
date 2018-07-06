﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Disorder.Unity.Core.Extensions.Await
{
    public class UnityCoroutineAwaiter<T> : INotifyCompletion
    {
        public bool IsCompleted { get; private set; }

        Exception exception;
        Action continueWith;
        T result;

        public T GetResult()
        {
            Helper.Assert(IsCompleted);

            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return result;
        }

        public void Complete(T result, Exception e)
        {
            Helper.Assert(!IsCompleted);

            IsCompleted = true;
            exception = e;
            this.result = result;

            if (continueWith != null)
            {
                UnityThreadingUtility.RunOnUnityScheduler(continueWith);
            }
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            Helper.Assert(continueWith == null || !IsCompleted);

            continueWith = continuation;
        }
    }

    public class UnityCoroutineAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; private set; }

        Exception exception;
        Action continueWith;

        public void GetResult()
        {
            Helper.Assert(IsCompleted);

            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }

        public void Complete(Exception e)
        {
            Helper.Assert(!IsCompleted);

            IsCompleted = true;
            exception = e;

            if (continueWith != null)
            {
                UnityThreadingUtility.RunOnUnityScheduler(continueWith);
            }
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            Helper.Assert(continueWith == null);
            Helper.Assert(!IsCompleted);

            continueWith = continuation;
        }
    }

    internal static class Helper
    {
        public static void Assert(bool condition)
        {
            if (condition)
                return;

            throw new Exception("Some condition is false in UnityCoroutineAwaiter...");
        }
    }
}