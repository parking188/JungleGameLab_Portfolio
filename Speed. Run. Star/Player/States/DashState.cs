using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;

    private Vector2 originalGravity;

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
        
        Controller.InitRigidData();
        Controller.rigidData.isDashBufferTime = true;
        Controller.rigidData.dashBufferTimer = 0f;
        Controller.rigidData.dashTimer = 0f;
        Controller.rigidData.dashDistance = Controller.rigidData.dashDistanceBlock * Controller.rigidData.blockSize;
        Controller.rigidData.dashVel = Controller.rigidData.dashDistance / Controller.rigidData.dashTime;

        Controller.rigidData.runVelocity = Vector2.zero;
        Controller.rigidData.jumpVelocity = Vector2.zero;
        Controller.rigidData.boostVelocity = Vector2.zero;
        Controller.rigidData.dashDirection = new Vector2(Controller.input.directionX, Controller.input.directionY);
        if(Controller.rigidData.dashDirection.y != 0f) { 
            Controller.rigidData.dashDirection.x = 0f;
            Controller.rigidData.dashAcc = -1 * Controller.rigidData.gravity.y;
        }
        Controller.rigidData.dashVelocity = Controller.rigidData.dashVel * Controller.rigidData.dashDirection;
        // originalGravity = Controller.rigidData.gravity;
        // Controller.rigidData.gravity = Vector2.zero;
        Controller.rigidData.isDashing = true;

        Controller.animator.SetBool("isDash", true);
    }

    public override void OnUpdateState()
    {
        if(CanStand() 
            || CanAir())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.dashTimer += Time.fixedDeltaTime;
        Controller.rigidData.dashVelocity += Controller.rigidData.dashAcc * Vector2.up;

    }
    public override void OnExitState()
    {
        if(Controller.rigidData.dashDirection.y < 0f){
            Controller.rigidData.tmpVelocity = Controller.rigidData.dashVelocity;
        }
        // Controller.rigidData.gravity = originalGravity;
        Controller.rigidData.dashVelocity = Vector2.zero;
        
        
        Controller.animator.SetBool("isDash", false);
    }

    private bool CanStand()
    {
        if(Controller.rigidData.dashTimer >= Controller.rigidData.dashTime && Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
            return true;
        }
        return false;
    }

    private bool CanAir()
    {
        if (Controller.rigidData.dashTimer >= Controller.rigidData.dashTime && !Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }
}
