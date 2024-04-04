using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public SpringState(PlayerController controller) : base(controller)
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
        Controller.rigidData.isSpring = true;
        Controller.rigidData.springTimer = 0f;
        Controller.rigidData.springGoalPosition = Controller.rigidData.springGoalPositionBlock * 8;
        Controller.rigidData.springHeigt = Controller.rigidData.springHeigtBlock * 8;
        Controller.rigidData.springStartVelY = 2 * Controller.rigidData.springHeigt / Controller.rigidData.springUpTime;
        Controller.rigidData.springUpAcc = -1 * (Controller.rigidData.springStartVelY / Controller.rigidData.springUpTime + Controller.rigidData.gravity.y);
        Controller.rigidData.springDownAcc = -2 * ((Controller.rigidData.springHeigt - Controller.rigidData.springGoalPosition.y) / (Controller.rigidData.springDownTime*Controller.rigidData.springDownTime) + Controller.rigidData.gravity.y);
        Controller.rigidData.springUpVelX = Controller.rigidData.springGoalPosition.x * Controller.rigidData.springUpTime / (Controller.rigidData.springUpTime + Controller.rigidData.springDownTime);
        Controller.rigidData.springDownVelX = Controller.rigidData.springGoalPosition.x * Controller.rigidData.springDownTime / (Controller.rigidData.springUpTime + Controller.rigidData.springDownTime);

        Controller.rigidData.springVelocity = new Vector2(Controller.rigidData.springUpVelX, Controller.rigidData.springStartVelY);
    }

    public override void OnUpdateState()
    {
        if(CanDash()
            || CanAir() 
            || CanStand())
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.springTimer += Time.fixedDeltaTime;
        if(Controller.rigidData.springTimer < Controller.rigidData.springUpTime){
            Controller.rigidData.springVelocity += Controller.rigidData.springUpAcc * Time.fixedDeltaTime * Vector2.up;
        }
        else if(Controller.rigidData.springTimer <  Controller.rigidData.springUpTime + Controller.rigidData.springDownTime) {
            if(Controller.rigidData.springVelocity.x == Controller.rigidData.springUpVelX){
                Controller.rigidData.springVelocity = new Vector2(Controller.rigidData.springDownVelX, 0);
            }
            Controller.rigidData.springVelocity += Controller.rigidData.springDownAcc * Time.fixedDeltaTime * Vector2.up;
        }
    }
    public override void OnExitState()
    {
        Controller.rigidData.springVelocity = Vector2.zero;
        Controller.rigidData.isSpring = false;
    }

    private bool CanAir()
    {
        if (Controller.rigidData.springTimer > Controller.rigidData.springUpTime + Controller.rigidData.springDownTime)
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }

    private bool CanStand()
    {
        if (Controller.rigidData.springTimer > Controller.rigidData.springUpTime + Controller.rigidData.springDownTime
            && Controller.ground.GetOnGround() 
            && Controller.rigidData.jumpVelocity.y <= 0f)
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
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
}
