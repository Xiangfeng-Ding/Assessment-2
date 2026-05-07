public class NpcStateMachine
{
    public NpcState currentState { get; private set; }

    public void Initialize(NpcState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(NpcState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }


}
