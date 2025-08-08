using System;

namespace Josh.StateMachines
{
    /// <summary>
    /// Predicate implementation that wraps a function
    /// </summary>
    public class FuncPredicate : IPredicate
    {
        private readonly Func<bool> _func;

        public FuncPredicate(Func<bool> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public bool Evaluate()
        {
            return _func();
        }
    }
}