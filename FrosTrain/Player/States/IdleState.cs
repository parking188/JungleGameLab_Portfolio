public class IdleState : PlayerState
{
    public IdleState(PlayerController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
    }

    public override void OnUpdateState()
    {
        //if (CanAir()
        //    || CanGrounded())
        //{
        //    return;
        //}

        //CameraLockOff();
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
    }

    //private bool CanAir()
    //{
    //    if (!Controller.IsGrounded)
    //    {
    //        Controller.player.stateMachine.ChangeState(StateName.Air);
    //        return true;
    //    }
    //    return false;
    //}

    //private void CameraLockOff()
    //{
    //    if (InputData.IsButtonOn(Controller.input.buttonsDown, InputData.BUTTONESCAPE) ||
    //        InputData.IsButtonOn(Controller.input.buttonsDown, InputData.BUTTONMOUSERIGHTCLICK))
    //    {
    //        Controller.CameraLockOff();
    //    }
    //}
}
