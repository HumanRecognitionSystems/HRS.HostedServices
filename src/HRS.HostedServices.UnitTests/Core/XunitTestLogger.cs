using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace HRS.HostedServices.UnitTests.Core
{
    public class XUnitTestLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;
        private IScope _scope;
        private string _name;

        public XUnitTestLogger(ITestOutputHelper outputHelper, string name)
        {
            _name = name;
            _outputHelper = outputHelper;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var scope = new Scope<TState>(this, state, _scope);
            _scope = scope;
            return scope;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper.WriteLine($"Name:{_name} Scope:{_scope} Level:{logLevel} EventId:{eventId} Message:{formatter(state, exception)}");
        }

        private interface IScope : IDisposable
        {}

        private class Scope<T> : IScope
        {
            private bool _isDisposed;
            private XUnitTestLogger _logger;
            private IScope _parent;
            private T _state;

            public Scope(XUnitTestLogger logger, T state, IScope parent)
            {
                _logger = logger;
                _parent = parent;
                _state = state;
            }

            public override string ToString()
            {
                return $"{_parent?.ToString()}:{_state?.ToString()}";
            }

            public void Dispose()
            {
                if(!_isDisposed)
                {
                    _isDisposed = true;
                    _logger._scope = _parent;
                }
            }
        }
    }

    public class XUnitTestLoggerNamed<T> : XUnitTestLogger, ILogger<T>
    {
        public XUnitTestLoggerNamed(ITestOutputHelper outputHelper) : base(outputHelper, typeof(T).Name)
        {
        }
    }
}
