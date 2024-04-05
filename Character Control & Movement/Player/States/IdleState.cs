using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class IdleState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public IdleState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        //Controller.anim.SetInteger("AnimationPar", 0);
    }

    public override void OnUpdateState()
    {
        if (CanMove()
            || CanJump())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.StopWhenLowSpeed();
    }

    public override void OnExitState()
    {
    }

    private bool CanMove()
    {
        if (Controller.input.direction != Vector3.zero)
        {
            Controller.player.stateMachine.ChangeState(StateName.Move);
            return true;
        }
        return false;
    }

    private bool CanJump()
    {
        if(InputData.IsButtonOn(Controller.input.buttonsDown, InputData.JUMPBUTTON)
            && Controller.isGrounded)
        {
            Controller.player.stateMachine.ChangeState(StateName.Jump);
            return true;
        }
        return false;
    }
}
