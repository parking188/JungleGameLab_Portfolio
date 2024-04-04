using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public StandState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.rigidData.runVelocity = Vector2.zero;
        Controller.rigidData.runDirection = 0;

        Controller.rigidData.jumpVelocity = Vector2.zero;
        Controller.rigidData.isJumping = false;
        Controller.rigidData.isAirJumping = false;
        Controller.rigidData.isDashing = false;
    }

    public override void OnUpdateState()
    {
        if(CanJump() 
            || CanDash()
            || CanRun() 
            || CanAir())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
    }

    public override void OnExitState()
    {
    }

    private bool CanJump()
    {
        if((Controller.input.buttonsDown & InputData.JUMPBUTTON) == InputData.JUMPBUTTON)
        {
            Controller.player.stateMachine.ChangeState(StateName.Jump);
            return true;
        }
        return false;
    }

    private bool CanRun()
    {
        if (Controller.input.directionX != 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Run);
            return true;
        }
        return false;
    }

    private bool CanAir()
    {
        if (!Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }

    private bool CanDash()
    {
        if ((Controller.input.buttonsDown & InputData.DASHBUTTON) == InputData.DASHBUTTON
            && !Controller.rigidData.isDashing
            && (Controller.input.directionX != 0f || Controller.input.directionY != 0f))
        {
            Controller.player.stateMachine.ChangeState(StateName.Dash);
            return true;
        }
        return false;
    }
}
