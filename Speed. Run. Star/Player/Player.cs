using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine stateMachine { get; private set; }
    public Animator animator { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }

    private PlayerController playerController {get; set;}

    // Status
    public void SetController(PlayerController _playerController){
        playerController = _playerController;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        stateMachine = new StateMachine(StateName.Stand, new StandState(playerController));
        InitStateMachine();
    }

    void Update()
    {
        stateMachine?.UpdateState();
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }

    public void OnUpdateStat(float moveSpeed, int dashCount)
    {

    }

    private void InitStateMachine()
    {
        stateMachine.AddState(StateName.Run, new RunState(playerController));
        stateMachine.AddState(StateName.Jump, new JumpState(playerController));
        stateMachine.AddState(StateName.Air, new AirState(playerController));
        stateMachine.AddState(StateName.Dash, new DashState(playerController));
        stateMachine.AddState(StateName.Boost, new BoostState(playerController));
        stateMachine.AddState(StateName.Trampoline, new TrampolineState(playerController));
        stateMachine.AddState(StateName.Spring, new SpringState(playerController));
    }
}