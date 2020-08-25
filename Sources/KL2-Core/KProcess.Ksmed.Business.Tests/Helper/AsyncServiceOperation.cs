using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KProcess.Ksmed.Business.Tests.Helper
{

    public class AsyncServiceOperationBase
    {
        protected ManualResetEvent Mre { get; private set; }
        protected Exception Exception { get; private set; }

        public AsyncServiceOperationBase()
        {
            Mre = new ManualResetEvent(false);
            this.OnError = e =>
            {
                Exception = e;
                Mre.Set();
            };
        }

        public void WaitCompletion()
        {
            Mre.WaitOne();
            if (Exception != null)
                Assert.Fail(Exception.ToString());
            Mre.Reset();
        }

        public Action<Exception> OnError { get; private set; }
    }

    public class AsyncServiceOperation : AsyncServiceOperationBase
    {
        public AsyncServiceOperation()
        {
            this.OnDone = () =>
            {
                Mre.Set();
            };
        }

        public new Action OnDone { get; private set; }
    }

    public class AsyncServiceOperation<T1> : AsyncServiceOperation
    {
        private T1 _results;
        public AsyncServiceOperation()
        {
            this.OnDone = a1 =>
            {
                _results = a1;
                Mre.Set();
            };
        }

        public new T1 WaitCompletion()
        {
            base.WaitCompletion();
            return _results;
        }

        public new Action<T1> OnDone { get; private set; }
    }

    public class AsyncServiceOperation<T1, T2> : AsyncServiceOperation
    {
        private Tuple<T1, T2> _results;
        public AsyncServiceOperation()
        {
            this.OnDone = (a1, a2) =>
            {
                _results = new Tuple<T1, T2>(a1, a2);
                Mre.Set();
            };
        }

        public new Tuple<T1, T2> WaitCompletion()
        {
            base.WaitCompletion();
            return _results;
        }

        public new Action<T1, T2> OnDone { get; private set; }
    }

    public class AsyncServiceOperation<T1, T2, T3> : AsyncServiceOperation
    {
        private Tuple<T1, T2, T3> _results;
        public AsyncServiceOperation()
        {
            this.OnDone = (a1, a2, a3) =>
            {
                _results = new Tuple<T1, T2, T3>(a1, a2, a3);
                Mre.Set();
            };
        }

        public new Tuple<T1, T2, T3> WaitCompletion()
        {
            base.WaitCompletion();
            return _results;
        }

        public new Action<T1, T2, T3> OnDone { get; private set; }
    }
}
