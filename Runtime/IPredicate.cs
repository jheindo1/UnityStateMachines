using System;

namespace Josh.StateMachines
{
    /// <summary>
    /// Interface for transition condition evaluation
    /// </summary>
    public interface IPredicate
    {
        bool Evaluate();
    }
    
    public class AndPredicate : IPredicate
    {
        private readonly IPredicate _first;
        private readonly IPredicate _second;

        public AndPredicate(IPredicate first, IPredicate second)
        {
            _first = first ?? throw new ArgumentNullException(nameof(first));
            _second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public bool Evaluate()
        {
            return _first.Evaluate() && _second.Evaluate();
        }
    }
    
    public class OrPredicate : IPredicate
    {
        private readonly IPredicate _first;
        private readonly IPredicate _second;

        public OrPredicate(IPredicate first, IPredicate second)
        {
            _first = first ?? throw new ArgumentNullException(nameof(first));
            _second = second ?? throw new ArgumentNullException(nameof(second));
        }

        public bool Evaluate()
        {
            return _first.Evaluate() || _second.Evaluate();
        }
    }

    public static class PredicateExtensions
    {
        public static IPredicate And(this IPredicate first, IPredicate second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            return new AndPredicate(first, second);
        }
        
        public static IPredicate Or(this IPredicate first, IPredicate second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            return new OrPredicate(first, second);
        }
    }
}