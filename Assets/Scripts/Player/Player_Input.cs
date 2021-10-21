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

    public bool controllerConnected = false;

    float inputDeadZone = 0.19f;

    PlayerInputMaster inputActions;

    private void Start()
    {
        inputActions = new PlayerInputMaster();
        inputActions.Player.Enable();

        inputActions.Player.Attack.performed += Attack_performed;
        inputActions.Player.Dash.performed += Dash_performed;
        inputActions.Player.Aim.performed += Aim_performed;
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Heal.performed += Heal_performed;
        inputActions.Player.Teleport.performed += Teleport_performed;

        inputActions.Player.Dash.canceled += Dash_canceled; ;
        inputActions.Player.Aim.canceled += Aim_canceled; ;
        inputActions.Player.Jump.canceled += Jump_canceled; ;

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        jumping = false;
        OnJumpUp();
    }

    private void Aim_canceled(InputAction.CallbackContext obj)
    {
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
            OnHeal(GameManager.instance.healingAmount);
        }
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        jumping = true;
        OnJumpDown();
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        if (controllerConnected)
        {
            if (directionalInput == Vector2.zero)
            {
                freeAimMode = true;
            }
        }
        else
        {
            OnTeleport();
        }
    }

    private void Dash_performed(InputAction.CallbackContext obj)
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

        if (!aiming && !freeAimMode)
        {
            Vector2 leftStickInput = inputActions.Player.Move.ReadValue<Vector2>();

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

       
        //if (Input.GetButtonDown("Jump"))
        //{
        //    jumping = true;
        //    OnJumpDown();
        //}

        //if (Input.GetButtonUp("Jump"))
        //{
        //    jumping = false;
        //    OnJumpUp();
        //}

        //if (Input.GetButtonDown("Dash"))
        //{
        //    OnDash();
        //}

        //if (Input.GetButtonUp("Dash"))
        //{
        //    OnDashUp();
        //}

        //if (!controllerConnected && Input.GetButtonDown("Aim"))
        //{
        //    OnBoomerangDash();
        //}
        //else if (controllerConnected && Input.GetButtonDown("Interact"))
        //{
        //    OnBoomerangDash();
        //}

        //if (Input.GetButtonDown("Heal"))
        //{
        //    if (GameManager.instance.healingPodAmount > 0)
        //    {
        //        OnHeal(GameManager.instance.healingAmount);
        //    }
        //}

        //if (Input.GetButtonDown("Attack") && CanAttack())
        //{
        //    nextAttackTime = Time.time + attackRate;
        //    OnAttack();
        //}
    }

    private void LeftStickInput()
    {
        Vector2 leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

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

    private void RightStickInput()
    {
        rightStickInput = new Vector2(Input.GetAxisRaw("RHorizontal"), Input.GetAxisRaw("RVertical"));
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
