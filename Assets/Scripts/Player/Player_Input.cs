using System;
using UnityEngine;

/// <summary>
/// Class responsible for handling player input. 
/// </summary>
public class Player_Input : MonoBehaviour
{
    public Vector2 directionalInput;
    public bool jumping { get; set; }
    public bool attacking { get; set; }
    public bool dashing { get; set; }
    public bool firing { get; set; }
    [SerializeField] private float fireRate = 0.1f;
    private float nextFireTime;

    public event Action OnFire = delegate{};
    public event Action OnAttack = delegate { };
    public event Action OnJumpUp = delegate { };
    public event Action OnJumpDown = delegate { };

    public int Xbox_One_Controller = 0;
    public int PS4_Controller = 0;

    private void Update()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                print("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                print("XBOX ONE CONTROLLER IS CONNECTED");
                PS4_Controller = 0;
                Xbox_One_Controller = 1;

            }
        }

        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetButtonDown("Jump"))
        {
            OnJumpDown();
        }
        if(Input.GetButtonUp("Jump"))
        {
            OnJumpUp();
        }
        if (Input.GetButtonDown("Attack"))
        {
            OnAttack();
        }
        firing = Input.GetButton("Fire");
        dashing = Input.GetButtonDown("Dash");
        if (firing && CanFire())
        {
            nextFireTime = Time.time + fireRate;
            OnFire();
        }
    }

    private bool CanFire()
    {
        return Time.time >= nextFireTime;
    }
    
}
