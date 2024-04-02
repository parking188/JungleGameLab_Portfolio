using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine<PlayerState, PlayerStateName> stateMachine { get; private set; }
    public PlayerController playerController { get; private set; }

    // Status
    public void SetController(PlayerController _playerController) => playerController = _playerController;

    public Train train;
    public Inventory inventory;

    void Awake()
    {

    }

    void Start()
    {
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
        stateMachine = new StateMachine<PlayerState, PlayerStateName>(PlayerStateName.Idle, new IdleState(playerController));
    }
}