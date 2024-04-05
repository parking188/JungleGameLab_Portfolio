using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public MoveState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        //Controller.anim.SetInteger("AnimationPar", 1);
    }

    public override void OnUpdateState()
    {
        if (CanDash()
            || CanJump()
            || CanIdle())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.RotateFixedUpdate();

        if (Controller.isGrounded)
            Controller.MoveFixedUpdate();
    }

    public override void OnExitState()
    {
        
    }

    private bool CanIdle()
    {
        if (Controller.isGrounded
            && Controller.input.direction == Vector3.zero)
        {
            Controller.ResetMoveVelocity();
            Controller.player.stateMachine.ChangeState(StateName.Idle);
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

    private bool CanDash()
    {
        if (InputData.IsButtonOn(Controller.input.buttonsDown, InputData.DASHBUTTON)
            && Controller.isGrounded)
        {
            Controller.ResetMoveVelocity();
            Controller.player.stateMachine.ChangeState(StateName.Dash);
            return true;
        }
        return false;
    }
}
