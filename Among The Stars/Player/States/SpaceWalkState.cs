using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWalkState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public SpaceWalkState(PlayerController controller) : base(controller)
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
        if(CanIdle())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        if(Controller.input.direction != Vector3.zero)
        {
            Vector3 forwardDirection = Controller.playerCamera.transform.forward;
            Vector3 rightDirection = Controller.playerCamera.transform.right;
            Vector3 moveDirection = (forwardDirection * Controller.input.direction.z + rightDirection * Controller.input.direction.x).normalized;
            float speed = ((Controller.input.buttons & InputData.DASHBUTTON) == InputData.DASHBUTTON) ? Controller.spaceWalkDashSpeed : Controller.spaceWalkSpeed;
            Controller.rigid.velocity += moveDirection * speed * Time.fixedDeltaTime;
        }
        else
        {
            Controller.rigid.velocity = Vector3.Lerp(Controller.rigid.velocity, Vector3.zero, Controller.spaceWalkStopSpeed * Time.fixedDeltaTime);
        }
    }

    public override void OnExitState()
    {
    }

    private bool CanIdle()
    {
        if (Controller.gravityControl.Gravity != null)
        {
            return true;
        }
        return false;
    }
}
