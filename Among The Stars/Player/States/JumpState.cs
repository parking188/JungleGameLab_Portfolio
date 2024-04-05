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

    }

    public override void OnUpdateState()
    {
        Controller.rigid.AddRelativeForce(0, Controller.jumpSpeed, 0, ForceMode.Impulse);
    }

    public override void OnFixedUpdateState()
    {

    }
    public override void OnExitState()
    {

    }

    private bool CanIdle()
    {
        //if (Controller.rigidData.jumpVelocity.y <= 0)
        //{
        //    Controller.player.stateMachine.ChangeState(StateName.Air);
        //    return true;
        //}
        return false;
    }
}
