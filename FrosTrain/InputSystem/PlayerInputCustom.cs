using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCustom : Singleton<PlayerInputCustom>
{
    private InputData input;
    private PlayerInputActionsCustom playerInput;

    private void Awake()
    {
        input = new InputData();
        playerInput = new PlayerInputActionsCustom();
    }

    void Start()
    {
        playerInput.Player.MoveHorizontal.started += OnMoveHorizontal;
        playerInput.Player.MoveHorizontal.canceled += OnMoveHorizontal;

        playerInput.Player.MoveVertical.started += OnMoveVertical;
        playerInput.Player.MoveVertical.canceled += OnMoveVertical;

        playerInput.Player.Move.started += OnMove;
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;

        playerInput.Player.Look.started += OnLook;
        playerInput.Player.Look.canceled += OnLook;

        playerInput.Player.ScrollWheel.started += OnScrollWheel;
        playerInput.Player.ScrollWheel.performed += OnScrollWheel;
        playerInput.Player.ScrollWheel.canceled += OnScrollWheel;

        playerInput.Player.Escape.started += OnButton;
        playerInput.Player.Escape.canceled += OnButton;

        playerInput.Player.MouseRightClick.started += OnButton;
        playerInput.Player.MouseRightClick.canceled += OnButton;

        playerInput.Player.Reservation.started += OnButton;
        playerInput.Player.Reservation.canceled += OnButton;

        playerInput.Player.ToggleTrainIcon.started += OnButton;
        playerInput.Player.ToggleTrainIcon.canceled += OnButton;

        playerInput.Player.SpaceBar.started += OnButton;
        playerInput.Player.SpaceBar.canceled += OnButton;

        playerInput.Player.Button1.started += OnButton;
        playerInput.Player.Button1.canceled += OnButton;

        playerInput.Player.Button2.started += OnButton;
        playerInput.Player.Button2.canceled += OnButton;

        playerInput.Player.Button3.started += OnButton;
        playerInput.Player.Button3.canceled += OnButton;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void GetInput(out InputData inputData)
    {
        inputData = input;
    }

    private void LateUpdate()
    {
        input.buttonsDown = 0x00;
        input.buttonsUp = 0x00;
    }

    public void OnMoveHorizontal(InputAction.CallbackContext context)
    {
        input.direction.x = context.ReadValue<float>();
    }

    public void OnMoveVertical(InputAction.CallbackContext context)
    {
        input.direction.z = context.ReadValue<float>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        input.direction.x = direction.x;
        input.direction.z = direction.y;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        input.look = context.ReadValue<Vector2>();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        input.scrollWheel = context.ReadValue<Vector2>();
    }

    public void OnButton(InputAction.CallbackContext context)
    {
        ButtonInputOnOff((byte)(int)(context.ReadValue<float>()), context.ReadValueAsButton());
    }

    public void ButtonInputOnOff(byte bitPosition, bool isOn)
    {
        if (isOn)
        {
            input.buttons |= bitPosition;
            input.buttonsDown |= bitPosition;
        }
        else
        {
            input.buttons &= (byte)(byte.MaxValue - bitPosition);
            input.buttonsUp |= bitPosition;
        }
    }
}
