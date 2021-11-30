using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Class responsible for handling player input. 
/// </summary>
public class Player_Input : MonoBehaviour
{
    public Vector2 directionalInput;
    public Vector2 rightStickInput;
    public Vector2 leftStickInputRaw;

    public bool jumping { get; set; }
    public bool attacking { get; set; }
    public bool aiming { get; set; }
    public bool freeAimMode;

    [SerializeField] private float attackRate = 0.1f;
    private float nextAttackTime;

    public event Action OnFire = delegate{};
    public event Action OnAim = delegate { };

    public event Action OnAttack = delegate { };
    public event Action OnJumpUp = delegate { };
    public event Action OnJumpDown = delegate { };
    public event Action OnDash = delegate { };
    public event Action OnDashUp = delegate { };

    public event Action OnTeleport = delegate { };
    public event Action<int> OnHeal = delegate { };

    public event Action MapOpen = delegate { };
    public event Action MapClose = delegate { };

    public bool controllerConnected = false;

    float inputDeadZone = 0.19f;

    PlayerInputMaster inputActions;
    PauseMenu pauseMenu;
    DialogManager dialogManager;

    private void Start()
    {
        inputActions = new PlayerInputMaster();
        inputActions.Player.Enable();
        inputActions.UI.Enable();

        dialogManager = DialogManager.instance;
        dialogManager.OnDialogueStart += DialogManager_OnDialogueStart;
        dialogManager.OnDialogueEnd += DialogManager_OnDialogueEnd;

        inputActions.Player.Attack.performed += Attack_performed;
        inputActions.Player.Dash.started += Dash_started;
        inputActions.Player.Aim.started += Aim_started;
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Heal.performed += Heal_performed;
        inputActions.Player.Teleport.performed += Teleport_performed;

        inputActions.UI.Pause.started += Pause_started;
        //inputActions.UI.Map.performed += Map_performed;
        //inputActions.UI.Map.canceled += Map_canceled;

        inputActions.Player.Dash.canceled += Dash_canceled; 
        inputActions.Player.Aim.canceled += Aim_canceled; 
        inputActions.Player.Jump.canceled += Jump_canceled; 

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;

        pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.OnPauseStart += PauseMenu_OnPauseStart;
        pauseMenu.OnPauseEnd += PauseMenu_OnPauseEnd;
        
    }

    //private void Map_canceled(InputAction.CallbackContext obj)
    //{
    //    MapClose();
    //}

    //private void Map_performed(InputAction.CallbackContext obj)
    //{
    //    MapOpen();
    //}

    private void PauseMenu_OnPauseEnd()
    {
        inputActions.Player.Enable();
    }

    private void PauseMenu_OnPauseStart()
    {
        inputActions.Player.Disable();
    }

    private void DialogManager_OnDialogueEnd()
    {
        inputActions.Player.Enable();
    }

    private void DialogManager_OnDialogueStart()
    {
        inputActions.Player.Disable();
    }

    private void Pause_started(InputAction.CallbackContext obj)
    {
        pauseMenu.TogglePause();
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        jumping = false;
        OnJumpUp();
    }

    private void Aim_canceled(InputAction.CallbackContext obj)
    {
        aiming = false;

        if (controllerConnected)
        {
            freeAimMode = false;
        }
    }

    private void Dash_canceled(InputAction.CallbackContext obj)
    {
        OnDashUp();
    }

    private void InputSystem_onDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log($"Device {device} was added");
                break;
            case InputDeviceChange.Removed:
                Debug.Log($"Device {device} was removed");
                break;
        }
    }

    private void Teleport_performed(InputAction.CallbackContext obj)
    {
        OnTeleport();
    }

    private void Heal_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.instance.healingPodAmount > 0)
        {
            OnHeal(GameManager.instance.healingAmountPerPod);
        }
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        jumping = true;
        OnJumpDown();
    }

    private void Aim_started(InputAction.CallbackContext obj)
    {
        if (controllerConnected)
        {
            if (directionalInput == Vector2.zero)
            {
                freeAimMode = true;
            }
        }
        else if(!aiming)
        {
            OnTeleport();
        }

        aiming = true;
    }

    private void Dash_started(InputAction.CallbackContext obj)
    {
        OnDash();
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        if (CanAttack())
        {
            nextAttackTime = Time.time + attackRate;
            OnAttack();
        }
    }


    private void Update()
    {
        if (Gamepad.all.Count > 0)
        {
            controllerConnected = true;
        }
        else
        {
            controllerConnected = false;
        }

        leftStickInputRaw = inputActions.Player.Move.ReadValue<Vector2>();

        if (!freeAimMode)
        {
            Vector2 leftStickInput = leftStickInputRaw;

            if (leftStickInput.magnitude < inputDeadZone)
            {
                leftStickInput = Vector2.zero;
            }
            else
            {
                leftStickInput = leftStickInput.normalized * ((leftStickInput.magnitude - inputDeadZone) / (1 - inputDeadZone));
            }
            directionalInput = new Vector2(Mathf.Round(leftStickInput.x), Mathf.Round(leftStickInput.y));
        }

        rightStickInput = inputActions.Player.Look.ReadValue<Vector2>();

        if (rightStickInput.magnitude < inputDeadZone)
        {
            rightStickInput = Vector2.zero;
        }
        else
        {
            rightStickInput = rightStickInput.normalized * ((rightStickInput.magnitude - inputDeadZone) / (1 - inputDeadZone));
        }
        rightStickInput = new Vector2(Mathf.Round(rightStickInput.x), Mathf.Round(rightStickInput.y));

       
    }

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

}
