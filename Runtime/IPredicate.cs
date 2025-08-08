namespace Josh.StateMachines
{
    /// <summary>
    /// Interface for transition condition evaluation
    /// </summary>
    public interface IPredicate
    {
        bool Evaluate();
    }
}