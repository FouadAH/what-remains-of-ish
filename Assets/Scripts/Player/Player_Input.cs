using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
/// <summary>
/// Class responsible for handling player input. 
/// </summary>
public class Player_Input : MonoBehaviour
{
    public Vector2 directionalInput;
    public Vector2 rightStickInput;
    public Vector2 leftStickInputRaw;
    public Vector2 rightStickInputRaw;
    public Vector2 mapRightStickInput;

    public bool jumping { get; set; }
    public bool attacking { get; set; }
    public bool aiming { get; set; }
    public bool freeAimMode;

    public float attackRate = 0.3f;
    public float attackRateFast = 0.2f;

    private float nextAttackTime;

    public event Action OnFire = delegate{};
    public event Action OnAim = delegate { };
    public event Action OnQuickThrow = delegate { };

    public event Action OnInteract = delegate { };
    public event Action OnAttack = delegate { };
    public event Action OnJumpUp = delegate { };
    public event Action OnJumpDown = delegate { };
    public event Action OnDash = delegate { };
    public event Action OnDashUp = delegate { };

    public event Action OnTeleport = delegate { };
    public event Action OnHeal = delegate { };

    public event Action MapOpen = delegate { };
    public event Action MapClose = delegate { };

    public event Action OnDialogueNext = delegate { };

    public event Action OnInputDeviceChanged = delegate { };

    public bool controllerConnected = false;

    float inputDeadZone = 0.19f;

    [HideInInspector] public PlayerInputMaster inputActions;

    public GameEvent submitClicked;
    public GameEvent inputChangedEvent;
    public GameEvent interactEvent;
    public GlobalConfigSO globalConfig;

    private void Awake()
    {
        inputActions = new PlayerInputMaster();

        if(startDisabled)
            inputActions.Player.Disable();
        else
            inputActions.Player.Enable();

        inputActions.UI.Enable();
        

        inputActions.Player.Interact.started += Interact_started;
        inputActions.Player.Attack.performed += Attack_performed;
        inputActions.Player.Dash.performed += Dash_started;
        inputActions.Player.Aim.started += Aim_started;
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Heal.performed += Heal_performed;
        inputActions.Player.Teleport.performed += Teleport_performed;

        inputActions.UI.DialogueNext.started += DialogueNext_started;
        inputActions.UI.Submit.started += Submit_started;

        inputActions.Player.Dash.canceled += Dash_canceled; 
        inputActions.Player.Aim.canceled += Aim_canceled; 
        inputActions.Player.Jump.canceled += Jump_canceled; 

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
        
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.started -= Interact_started;
        inputActions.Player.Attack.performed -= Attack_performed;
        inputActions.Player.Dash.performed -= Dash_started;
        inputActions.Player.Aim.started -= Aim_started;
        inputActions.Player.Jump.performed -= Jump_performed;
        inputActions.Player.Heal.performed -= Heal_performed;
        inputActions.Player.Teleport.performed -= Teleport_performed;

        inputActions.UI.DialogueNext.started -= DialogueNext_started;
        inputActions.UI.Submit.started -= Submit_started;

        inputActions.Player.Dash.canceled -= Dash_canceled;
        inputActions.Player.Aim.canceled -= Aim_canceled;
        inputActions.Player.Jump.canceled -= Jump_canceled;

        InputSystem.onDeviceChange -= InputSystem_onDeviceChange;
    }

    private void Submit_started(InputAction.CallbackContext obj)
    {
        submitClicked.Raise();
    }

    private void Interact_started(InputAction.CallbackContext obj)
    {
        interactEvent.Raise();
    }

    private void DialogueNext_started(InputAction.CallbackContext obj)
    {
        OnDialogueNext();
    }

    private void QuickThrow_started(InputAction.CallbackContext obj)
    {
        OnQuickThrow();
    }

    bool startDisabled;

    public void DisablePlayerInput()
    {
        if (inputActions == null)
        {
            startDisabled = true;
        }
        else
        {
            inputActions.Player.Disable();
        }
    }

    public void EnablePlayerInput()
    {
        inputActions.Player.Enable();
    }

    public void OnRestStart()
    {
        inputActions.Player.Disable();
    }
    public void OnRestEnd()
    {
        inputActions.Player.Enable();
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

        controllerConnected = Gamepad.all.Count > 0;
        globalConfig.gameSettings.controllerConnected = controllerConnected;
        inputChangedEvent.Raise();
    }

    private void Teleport_performed(InputAction.CallbackContext obj)
    {
        OnTeleport();
    }

    private void Heal_performed(InputAction.CallbackContext obj)
    {
        OnHeal();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        jumping = true;
        OnJumpDown();
    }

    private void Aim_started(InputAction.CallbackContext obj)
    {
        if (obj.interaction is HoldInteraction)
        {
            if (controllerConnected)
            {
                if (directionalInput == Vector2.zero)
                {
                    //freeAimMode = true;
                }
            }
            //else if (!aiming)
            //{
            //    OnTeleport();
            //}

            aiming = true;
        }
        //else
        //{
        //    OnQuickThrow();
        //    Debug.Log("Tap");

        //}
    }

    private void Dash_started(InputAction.CallbackContext obj)
    {
        OnDash();
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        if (CanAttack())
        {
            float tempAttackRate = (GameManager.instance.equippedBrooch_01) ? attackRateFast : attackRate;
            nextAttackTime = Time.time + tempAttackRate;
            OnAttack();
        }
        //OnAttack();
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

        mapRightStickInput = inputActions.UI.MapMovement.ReadValue<Vector2>();
        leftStickInputRaw = inputActions.Player.Move.ReadValue<Vector2>();
        rightStickInputRaw = inputActions.Player.Look.ReadValue<Vector2>();

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

        rightStickInput = inputActions.Player.CameraLook.ReadValue<Vector2>();

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
