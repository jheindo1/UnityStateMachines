namespace Josh.StateMachines
{
    /// <summary>
    /// Interface for state machine states
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called when entering the state
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// Called when exiting the state
        /// </summary>
        void OnExit();
    }
}