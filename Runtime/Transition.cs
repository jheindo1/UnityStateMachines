using System;

namespace Josh.StateMachines
{
    /// <summary>
    /// Represents a state transition with a target state and condition
    /// </summary>
    public class Transition
    {
        public IState TargetState { get; }
        public IPredicate Predicate { get; }

        public Transition(IState targetState, IPredicate predicate)
        {
            TargetState = targetState ?? throw new ArgumentNullException(nameof(targetState));
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }
    }
}