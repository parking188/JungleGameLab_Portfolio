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
    }

    public override void OnUpdateState()
    {
        if (CanIdle())
        {
            Controller.player.stateMachine.ChangeState(StateName.Idle);
        }
    }

    public override void OnFixedUpdateState()
    {
        //Controller.transform.Rotate(0, Controller.input.direction.x * Controller.spaceWalkSpeed * 100f, 0);
        //Controller.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Controller.transform.Translate(0, 0, Controller.input.direction.z * Controller.spaceWalkSpeed * Time.deltaTime);
    }

    public override void OnExitState()
    {
    }

    private bool CanIdle()
    {
        if (Controller.input.direction == Vector3.zero)
        {
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
}
