using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public RunState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.rigidData.jumpVelocity = Vector2.zero;
        Controller.rigidData.isJumping = false;
        Controller.rigidData.isAirJumping = false;
        Controller.rigidData.isDashing = false;
    }

    public override void OnUpdateState()
    {
        if(CanJump() 
            || CanTrampoline()
            || CanSpring()
            || CanBoost()
            || CanStand()
            || CanAir())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.runDirection = Controller.input.directionX;

        if (Controller.rigidData.runAccelerationTime == 0)
        {
            Controller.rigidData.runVelocity = Controller.rigidData.runSpeed * Controller.rigidData.runDirection * Vector2.right;
            Controller.rigidData.runAcc = 0f;
        }
        else
        {
            Controller.rigidData.runAcc = Controller.rigidData.runSpeed * Controller.rigidData.runDirection / Controller.rigidData.runAccelerationTime;
        }

        if (Controller.rigidData.runDecelerationTime == 0)
        {
            Controller.rigidData.runDeAcc = 0f;
        }
        else
        {
            Controller.rigidData.runDeAcc = Controller.rigidData.runSpeed * Controller.rigidData.runDirection / Controller.rigidData.runDecelerationTime * -1;
        }

        if (Controller.input.directionX != 0)
        {
            if (Controller.rigidData.runVelocity.x * Controller.rigidData.runDirection < Controller.rigidData.runSpeed)
            {
                Controller.rigidData.runVelocity += Controller.rigidData.runAcc * Time.fixedDeltaTime * Vector2.right;
            }
            else
            {
                Controller.rigidData.runVelocity = Controller.rigidData.runSpeed * Controller.rigidData.runDirection * Vector2.right;
            }
        }
        else
        {
            if (Controller.rigidData.runDecelerationTime == 0)
            {
                Controller.rigidData.runVelocity = Vector2.zero;
            }

            if (Controller.rigidData.runVelocity.x * Controller.rigidData.runDirection > 0)
            {
                Controller.rigidData.runVelocity += Controller.rigidData.runDeAcc * Time.fixedDeltaTime * Vector2.right;
            }
        }
    }
    public override void OnExitState()
    {
    }

    private bool CanJump()
    {
        if ((Controller.input.buttonsDown & InputData.JUMPBUTTON) == InputData.JUMPBUTTON)
        {
            Controller.player.stateMachine.ChangeState(StateName.Jump);
            return true;
        }
        return false;
    }

    private bool CanStand()
    {
        if (Controller.ground.GetOnGround()
            && Controller.input.directionX == 0
            && (Controller.rigidData.runVelocity.x * Controller.rigidData.runDirection <= 0))
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
            return true;
        }
        return false;
    }

    private bool CanAir()
    {
        if (!Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
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
