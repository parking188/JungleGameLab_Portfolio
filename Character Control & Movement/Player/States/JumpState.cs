using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public JumpState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.Jump();
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
        Controller.RotateFixedUpdate();
        Controller.MoveFixedUpdate();
    }

    public override void OnExitState()
    {
        
    }

    private bool CanMove()
    {
        if (Controller.isGrounded
            && Controller.YVelocity.y <= 0f
            && Controller.input.direction != Vector3.zero)
        {
            Controller.player.stateMachine.ChangeState(StateName.Move);
            return true;
        }
        return false;
    }

    private bool CanIdle()
    {
        if (Controller.isGrounded
            && Controller.YVelocity.y <= 0f)
        {
            Controller.ResetMoveVelocity();
            Controller.ResetRigidXZVelocity();
            Controller.player.stateMachine.ChangeState(StateName.Idle);
            return true;
        }
        return false;
    }
}
