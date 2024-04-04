using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public AirState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.rigidData.isLandingBoost = false;
    }

    public override void OnUpdateState()
    {
        if(
            CanBoost()
            ||CanJump() 
            || CanDash() 
            || CanTrampoline()
            || CanSpring()
            || CanRun()
            || CanStand()
            )
        {
            return;
        }

        ChangeColorWhenLandingBoost();
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.runDirection = Controller.input.directionX;
        Controller.rigidData.runVelocity = Controller.rigidData.airMoving 
            * Controller.rigidData.blockSize 
            * Controller.rigidData.runDirection 
            * Controller.rigidData.airMoving 
            * Vector2.right;
    }
    public override void OnExitState()
    {
        Controller.ChangeBodyColor(Color.white);
    }

    private void ChangeColorWhenLandingBoost()
    {
        if(Controller.spriteRenderer.color == Color.white && Controller.ground.GetOnLandingDashGround())
        {
            Controller.ChangeBodyColor(Color.green);
        }
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

    private bool CanRun()
    {
        if (Controller.ground.GetOnGround() 
            && Controller.input.directionX != 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Run);
            return true;
        }
        return false;
    }

    private bool CanStand()
    {
        if (Controller.ground.GetOnGround() 
            && Controller.rigid.velocity.y <= 0f)
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
        else if(Controller.rigidData.isLandingBoost && Controller.ground.GetOnGround())
        {
            Controller.SetBoostOn(Controller.rigidData.landingBoostVelocityPerBlock, 
                Controller.rigidData.landingBoostTime, 
                Controller.rigidData.landingBoostAccelerationTime, 
                Controller.rigidData.landingBoostDecelerationTime);
            Controller.rigidData.canGetStar = true;
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
        if(Controller.rigidData.isSpring)
        {
            Controller.player.stateMachine.ChangeState(StateName.Spring);
            return true;
        }
        return false;
    }
}
