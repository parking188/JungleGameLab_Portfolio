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
        Controller.rigidData.jumpHeight = Controller.rigidData.jumpHeightPerBlock * Controller.rigidData.runSpeedPerBlock;
        Controller.rigidData.jumpUpAcc = -2 * Controller.rigidData.jumpHeight / (Controller.rigidData.jumpUpTime * Controller.rigidData.jumpUpTime) - Controller.rigidData.gravity.y;
        Controller.rigidData.jumpStartVel = 2 * Controller.rigidData.jumpHeight / Controller.rigidData.jumpUpTime;
        Controller.rigidData.jumpVelocity = Controller.rigidData.jumpStartVel * Vector2.up;

        if (Controller.rigidData.jumpVelocity.y <= 0)
        {
            Controller.rigidData.jumpVelocity = Vector2.zero;
        }

        if (!Controller.rigidData.isJumping)
        {
            Controller.rigidData.isJumping = true;
        }
        else
        {
            Controller.rigidData.isAirJumping = true;
        }
    }

    public override void OnUpdateState()
    {
        if(CanJump() 
            || CanDash() 
            || CanTrampoline()
            || CanSpring()
            || CanBoost()
            || CanStand() 
            || CanAir()
            )
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.runDirection = Controller.input.directionX;
        // RunUpdate();
        Controller.rigidData.runVelocity = Controller.rigidData.airMoving 
            * Controller.rigidData.blockSize 
            * Controller.rigidData.runDirection 
            * Controller.rigidData.airMoving 
            * Vector2.right;

        Controller.rigidData.jumpVelocity += Controller.rigidData.jumpUpAcc * Time.fixedDeltaTime * Vector2.up;
    }
    public override void OnExitState()
    {
        Controller.rigidData.jumpVelocity = Vector2.zero;
    }

    private bool CanAir()
    {
        if (Controller.rigidData.jumpVelocity.y <= 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }
    private bool CanJump()
    {
        if ((Controller.input.buttonsDown & InputData.JUMPBUTTON) == InputData.JUMPBUTTON
            && !Controller.rigidData.isAirJumping)
        {
            Controller.player.stateMachine.ChangeState(StateName.Jump);
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

    private bool CanStand()
    {
        if (Controller.ground.GetOnGround() && Controller.rigidData.jumpVelocity.y <= 0f)
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
            return true;
        }
        return false;
    }
    
    private bool CanBoost()
    {
        if (Controller.rigidData.isBoosting)
        {
            Controller.player.stateMachine.ChangeState(StateName.Boost);
            return true;
        }
        return false;
    }
    
    private bool CanTrampoline()
    {
        if (Controller.rigidData.isTrampolining)
        {
            Controller.player.stateMachine.ChangeState(StateName.Trampoline);
            return true;
        }
        return false;
    }

    private bool CanSpring()
    {
        if (Controller.rigidData.isSpring)
        {
            Controller.player.stateMachine.ChangeState(StateName.Spring);
            return true;
        }
        return false;
    }
}
