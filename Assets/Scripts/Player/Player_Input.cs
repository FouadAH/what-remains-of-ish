using System;
using UnityEngine;

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

    public event Action OnBoomerangDash = delegate { };
    public event Action<int> OnHeal = delegate { };

    public event Action OnDetach = delegate { };
    public event Action OnAttach = delegate { };

    public bool controllerConnected = false;

    float inputDeadZone = 0.19f;
    private void Update()
    {
        string[] names = Input.GetJoystickNames();
        controllerConnected = false;
        foreach (string item in names)
        {
            if (!item.Equals(""))
            {
                controllerConnected = true;
            }
        }

        if (controllerConnected && Input.GetButtonDown("Aim") && directionalInput == Vector2.zero)
        {
            freeAimMode = true;
        }
        else if (Input.GetButtonUp("Aim"))
        {
            freeAimMode = false;
        }

        if (!aiming && !freeAimMode)
        {
            LeftStickInput();
        }

        RightStickInput();

        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
            OnJumpDown();
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumping = false;
            OnJumpUp();
        }

        if (Input.GetButtonDown("Dash"))
        {
            OnDash();
        }

        if (Input.GetButtonUp("Dash"))
        {
            OnDashUp();
        }

        if (!controllerConnected && Input.GetButtonDown("Aim"))
        {
            OnBoomerangDash();
        }
        else if (controllerConnected && Input.GetButtonDown("Interact"))
        {
            OnBoomerangDash();
        }

        if (Input.GetButtonDown("Heal"))
        {
            if (GameManager.instance.healingPodAmount > 0)
            {
                OnHeal(GameManager.instance.healingAmount);
            }
        }

        attacking = Input.GetButtonDown("Attack");
        if (attacking && CanAttack())
        {
            nextAttackTime = Time.time + attackRate;
            OnAttack();

        }
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
