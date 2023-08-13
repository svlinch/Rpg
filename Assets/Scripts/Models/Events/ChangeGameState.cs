public class ChangeGameState
{
    public EState State;
    public ChangeGameState(EState state)
    {
        State = state;
    }
}

public enum EState
{
    Left,
    Right
}
