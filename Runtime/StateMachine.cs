using System;
using System.Collections.Generic;
using UnityEngine;

namespace Josh.StateMachines
{
    /// <summary>
    /// A generic finite state machine implementation for Unity projects.
    /// Supports state-specific and global transitions with predicate-based conditions.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Event fired when a state change occurs. Parameters: (previousState, newState)
        /// </summary>
        public event Action<IState, IState> OnStateChanged;

        private IState _currentState;
        private readonly Dictionary<IState, List<Transition>> _transitions = new Dictionary<IState, List<Transition>>();
        private readonly List<Transition> _anyTransitions = new List<Transition>();

        /// <summary>
        /// Gets the current active state
        /// </summary>
        public IState CurrentState => _currentState;

        /// <summary>
        /// Creates a new state machine with the specified starting state
        /// </summary>
        /// <param name="startState">The initial state (cannot be null)</param>
        public StateMachine(IState startState)
        {
            _currentState = startState;
            if (_currentState != null)
            {
                _currentState.OnEnter();
            }
        }

        /// <summary>
        /// Updates the state machine, checking for transitions and updating the current state
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        public void Update(float deltaTime)
        {
            bool didTransition = false;
            
            if (_currentState != null)
            {
                // Check for transitions from the current state (higher priority than any-transitions)
                if (_transitions.TryGetValue(_currentState, out var transitions))
                {
                    foreach (var transition in transitions)
                    {
                        if (transition.Predicate.Evaluate())
                        {
                            ChangeState(transition.TargetState);
                            didTransition = true;
                            break; // Exit after a successful transition
                        }
                    }
                }
            }

            if (!didTransition)
            {
                // Check for any transitions (lower priority)
                foreach (var transition in _anyTransitions)
                {
                    if (transition.Predicate.Evaluate())
                    {
                        ChangeState(transition.TargetState);
                        didTransition = true;
                        break; // Exit after a successful transition
                    }
                }
            }

            // Update current state
            _currentState?.OnUpdate(deltaTime);
        }

        /// <summary>
        /// Forces an immediate state change without checking transition conditions
        /// </summary>
        /// <param name="newState">The state to transition to</param>
        public void ForceState(IState newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("Attempted to force transition to null state");
                return;
            }
            
            ChangeState(newState);
        }

        private void ChangeState(IState newState)
        {
            if (newState == null)
            {
                Debug.LogError("Attempted to transition to null state");
                return;
            }

            var previousState = _currentState;
            
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
            
            OnStateChanged?.Invoke(previousState, newState);
        }

        /// <summary>
        /// Adds a transition from one state to another with a predicate condition
        /// </summary>
        /// <param name="fromState">Source state</param>
        /// <param name="toState">Target state</param>
        /// <param name="predicate">Condition for the transition</param>
        public void AddTransition(IState fromState, IState toState, IPredicate predicate)
        {
            if (fromState == null) throw new ArgumentNullException(nameof(fromState));
            if (toState == null) throw new ArgumentNullException(nameof(toState));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (!_transitions.ContainsKey(fromState))
            {
                _transitions[fromState] = new List<Transition>();
            }
            _transitions[fromState].Add(new Transition(toState, predicate));
        }

        /// <summary>
        /// Adds a global transition that can trigger from any state
        /// </summary>
        public void AddAnyTransition(IState toState, IPredicate predicate, bool allowSelfTransition = false)
        {
            if (toState == null) throw new ArgumentNullException(nameof(toState));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            IPredicate newPredicate = predicate;
            if (!allowSelfTransition)
            {
                newPredicate = predicate.And(new FuncPredicate(()=> toState != _currentState));
            }
            
            _anyTransitions.Add(new Transition(toState, newPredicate));
        }
        
        /// <summary>
        /// Adds a transition using a function as the predicate condition
        /// </summary>
        public void AddTransition(IState fromState, IState toState, Func<bool> predicateFunc)
        {
            if (predicateFunc == null) throw new ArgumentNullException(nameof(predicateFunc));
            AddTransition(fromState, toState, new FuncPredicate(predicateFunc));
        }
        
        /// <summary>
        /// Adds a global transition using a function as the predicate condition
        /// </summary>
        public void AddAnyTransition(IState toState, Func<bool> predicateFunc, bool allowSelfTransition = false)
        {
            if (predicateFunc == null) throw new ArgumentNullException(nameof(predicateFunc));
            
            IPredicate predicate = new FuncPredicate(predicateFunc);
            if (!allowSelfTransition)
            {
                predicate = predicate.And(new FuncPredicate(() => toState != _currentState));
            }
            
            AddAnyTransition(toState, predicate);
        }

        /// <summary>
        /// Removes all transitions from a specific state
        /// </summary>
        public void RemoveTransitionsFrom(IState fromState)
        {
            _transitions.Remove(fromState);
        }
        
        public void RemoveTransitionsTo(IState toState)
        {
            foreach (var transitions in _transitions.Values)
            {
                transitions.RemoveAll(t => t.TargetState == toState);
            }
            _anyTransitions.RemoveAll(t => t.TargetState == toState);
        }

        /// <summary>
        /// Clears all transitions (useful for cleanup or reconfiguration)
        /// </summary>
        public void ClearAllTransitions()
        {
            _transitions.Clear();
            _anyTransitions.Clear();
        }
    }
}