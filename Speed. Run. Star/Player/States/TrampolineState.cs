using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public TrampolineState(PlayerController controller) : base(controller)
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
        Controller.rigidData.trampolineDirectionNorm = Controller.rigidData.trampolineDirection.normalized;
        Controller.rigidData.trampolineStartVelocity = 2 * Controller.rigidData.trampolineDistance / Controller.rigidData.trampolineTime;
        Controller.rigidData.trampolineVelocity = Controller.rigidData.trampolineStartVelocity * Controller.rigidData.trampolineDirectionNorm;
        Controller.rigidData.trampolineDeAcc = new Vector2(-1 * Controller.rigidData.trampolineVelocity.x * Controller.rigidData.trampolineDirectionNorm.x / Controller.rigidData.trampolineTime, -1 * (Controller.rigidData.trampolineVelocity.y * Controller.rigidData.trampolineDirectionNorm.y / Controller.rigidData.trampolineTime + Controller.rigidData.gravity.y));
        
        Controller.rigidData.isTrampolining = false;
    }

    public override void OnUpdateState()
    {
        if(CanJump() 
            || CanDash()
            || CanTrampoline()
            || CanSpring()
            || CanRun()
            || CanStand() 
            || CanAir())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.trampolineTimer += Time.fixedDeltaTime;
        if(Controller.rigidData.trampolineTimer < Controller.rigidData.trampolineTime){
            Controller.rigidData.trampolineVelocity += Controller.rigidData.trampolineDeAcc * Time.fixedDeltaTime * Controller.rigidData.trampolineDirectionNorm;
        }
    }

    public override void OnExitState()
    {
        Controller.rigidData.trampolineVelocity = Vector2.zero;
        Controller.rigidData.trampolineTimer = 0;
        Controller.rigidData.isTrampolining = false;
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


    private bool CanAir()
    {
        if (Controller.rigidData.trampolineTimer >= Controller.rigidData.trampolineTime
            && !Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }
    private bool CanStand()
    {
        if (Controller.rigidData.trampolineTimer >= Controller.rigidData.trampolineTime
            && Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
            return true;
        }
        return false;
    }
    private bool CanRun()
    {
        if (Controller.rigidData.trampolineTimer >= Controller.rigidData.trampolineTime
            && Controller.ground.GetOnGround()
            && Controller.input.directionX != 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Run);
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

    private bool CanTrampoline()
    {
        if(Controller.rigidData.isTrampolining)
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
