using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BoostState : BaseState
{
    // Animation
    //public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    //private int hashMoveAnimation;


    public BoostState(PlayerController controller) : base(controller)
    {
        //hashMoveAnimation = Animator.StringToHash("Velocity");
    }

    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        return 0.0f;
    }

    public override void OnEnterState()
    {
        Controller.ChangeBodyColor(Color.green);
        Controller.StopCoroutine("DecelerationBoost");
        Controller.rigidData.boostVelocity = Vector2.zero;
        if(Controller.rigidData.boostAccelerationTime > Controller.rigidData.boostTime){ Debug.Assert(false, "Error Controller.rigidData.boostAccelerationTime must small than Controller.rigidData.boostTime"); }
        Controller.rigidData.boostDirection = Controller.input.directionX;
        Controller.rigidData.boostSpeed = Controller.rigidData.boostVelocityPerBlock * Controller.rigidData.blockSize;
        if(Controller.rigidData.boostAccelerationTime == 0){
            Controller.rigidData.boostVelocity = Controller.rigidData.boostSpeed * Controller.rigidData.boostDirection * Vector2.right;
            Controller.rigidData.boostAcc = 0f;
        }
        else{
            Controller.rigidData.boostAcc = Controller.rigidData.boostSpeed / Controller.rigidData.boostAccelerationTime;
        }

        if(Controller.rigidData.boostDirection > 0){
            Controller.BoostEffectRight.Play();
        }
        else if(Controller.rigidData.boostDirection < 0){
            Controller.BoostEffectLeft.Play();
        }

        
        if(!Controller.animator.GetBool("isBoost")){
            Controller.animator.SetBool("isBoost", true);
        }
    }

    public override void OnUpdateState()
    {
        if(CanJump() 
            || CanDash() 
            || CanTrampoline()
            || CanSpring()
            || CanRun()
            || CanStand()
            || CanAir()
            )
        {
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        Controller.rigidData.boostTimer += Time.fixedDeltaTime;
        if(Controller.rigidData.boostTimer < Controller.rigidData.boostTime){
            if(Controller.rigidData.boostTimer < Controller.rigidData.boostAccelerationTime){
                Controller.rigidData.boostVelocity += Controller.rigidData.boostAcc * Time.fixedDeltaTime * Controller.rigidData.boostDirection * Vector2.right;
            }
        }
        else{
            Controller.rigidData.boostTimer = 0;
            Controller.rigidData.isBoosting = false;
        }
    }
    public override void OnExitState()
    {
        if(Controller.rigidData.boostDecelerationTime != 0){
            Controller.rigidData.boostDeAcc = -1 * Controller.rigidData.boostVelocity.x / Controller.rigidData.boostDecelerationTime;
        }
        else{
            Controller.rigidData.boostVelocity = Vector2.zero;
        }
        Controller.rigidData.isBoosting = false;

        Controller.BoostEffectRight.Stop();
        Controller.BoostEffectLeft.Stop();

        Controller.ChangeBodyColor(Color.white);

        Controller.animator.SetBool("isBoost", false);
        
        Controller.StartCoroutine(DecelerationBoost());


        Controller.rigidData.canGetStar = false;

    }
    IEnumerator DecelerationBoost(){
        while(Controller.rigidData.boostVelocity != Vector2.zero) {
            Controller.rigidData.boostVelocity += Controller.rigidData.boostDeAcc * Time.fixedDeltaTime * Vector2.right;
            if(Controller.rigidData.boostVelocity.x * Controller.rigidData.boostDirection <= 0) Controller.rigidData.boostVelocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
        }
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
        if (!Controller.rigidData.isBoosting
            && Controller.ground.GetOnGround()
            && Controller.input.directionX == 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Stand);
            return true;
        }
        return false;
    }

    private bool CanAir()
    {
        if (!Controller.rigidData.isBoosting
            && !Controller.ground.GetOnGround())
        {
            Controller.player.stateMachine.ChangeState(StateName.Air);
            return true;
        }
        return false;
    }

    
    private bool CanRun()
    {
        if (!Controller.rigidData.isBoosting
            && Controller.ground.GetOnGround() 
            && Controller.input.directionX != 0)
        {
            Controller.player.stateMachine.ChangeState(StateName.Run);
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
