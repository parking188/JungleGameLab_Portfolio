using System.Collections;
using System.Collections.Generic;
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
    }

    public override void OnUpdateState()
    {
        if (CanSpaceWalk()
            ||CanMove())
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
        //if((Controller.input.buttonsDown & InputData.JUMPBUTTON) == InputData.JUMPBUTTON)
        //{
        //    Controller.player.stateMachine.ChangeState(StateName.Jump);
        //    return true;
        //}
        return false;
    }

    private bool CanSpaceWalk()
    {
        if (Controller.gravityControl.Gravity == null)
        {
            Controller.player.stateMachine.ChangeState(StateName.SpaceWalk);
            return true;
        }
        return false;
    }
}
