using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DashState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public DashState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.Dash();
    }

    public override void OnUpdateState()
    {
        if (CanMove()
            || CanIdle())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.ResetGravityVelocity();
    }

    public override void OnExitState()
    {
        
    }

    private bool CanIdle()
    {
        if (Controller.animator.GetBool("Dash") == false
            && Controller.isGrounded
            && Controller.input.direction == Vector3.zero)
        {
            Controller.ResetMoveVelocity();
            Controller.player.stateMachine.ChangeState(StateName.Idle);
            return true;
        }
        return false;
    }

    private bool CanMove()
    {
        if (Controller.animator.GetBool("Dash") == false
            && Controller.isGrounded
            && Controller.input.direction != Vector3.zero)
        {
            Controller.player.stateMachine.ChangeState(StateName.Move);
            return true;
        }
        return false;
    }
}
