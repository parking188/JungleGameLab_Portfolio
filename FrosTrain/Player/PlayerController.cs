using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : Singleton<PlayerController>
{
    [HideInInspector] public Player player;
    [HideInInspector] public InputData input;

    public DragObject dragObject;
    public TrainCar selectedTrainCar;
    public InventoryCard selectedCard;
    public TrainCar dropOnCar;

    private void Awake()
    {
        player = GetComponent<Player>();
        player.SetController(this);

        input = new InputData();
    }

    private void Start()
    {
        if (dragObject == null) dragObject = GameObject.Find("UICanvas").transform.Find("DragObject").GetComponent<DragObject>();
        dragObject.InitDragObject();
    }

    private void Update()
    {
        PlayerInputCustom.Instance.GetInput(out input);

        if (InputData.IsButtonOn(input.buttonsDown, InputData.BUTTONTOGGLETRAINICON)) player.train.ToggleAllIcon();

        if (InputData.IsButtonOn(input.buttonsDown, InputData.BUTTONSPACEBAR)) GameManager.Instance.TimeStopByPlayerToggle();
        if (InputData.IsButtonOn(input.buttonsDown, InputData.BUTTON1)) GameManager.Instance.OnFastForward(2f, EPlayStatus.X2);
        if (InputData.IsButtonOn(input.buttonsUp, InputData.BUTTON1)) GameManager.Instance.OffFastForward(EPlayStatus.X2);
        if (InputData.IsButtonOn(input.buttonsDown, InputData.BUTTON2) && SaveManager.Instance.gameClearCount >= 1) GameManager.Instance.OnFastForward(5f, EPlayStatus.X5);
        if (InputData.IsButtonOn(input.buttonsUp, InputData.BUTTON2) && SaveManager.Instance.gameClearCount >= 1) GameManager.Instance.OffFastForward(EPlayStatus.X5);
        if (InputData.IsButtonOn(input.buttonsDown, InputData.BUTTON3) && SaveManager.Instance.gameClearCount >= 3) GameManager.Instance.OnFastForward(10f, EPlayStatus.X10);
        if (InputData.IsButtonOn(input.buttonsUp, InputData.BUTTON3) && SaveManager.Instance.gameClearCount >= 3) GameManager.Instance.OffFastForward(EPlayStatus.X10);
    }

    public void ChangeState(PlayerStateName state)
    {
        player.stateMachine.ChangeState(state);
    }

    public bool CompareState(Type type)
    {
        return player.stateMachine.CurrentState.GetType() == type;
    }

    public void SelectCar(TrainCar car)
    {
        selectedTrainCar = car;
    }

    //public void CameraLockOff()
    //{
    //    player.train.CameraLockOff();
    //    selectedCar = null;
    //}
}