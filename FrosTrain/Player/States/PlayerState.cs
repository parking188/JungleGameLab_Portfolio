public abstract class PlayerState : BaseState
{
    protected PlayerController Controller { get; private set; }
    public PlayerState(PlayerController controller)
    {
        this.Controller = controller;
    }
}

public enum PlayerStateName
{
    Idle = 0
}
