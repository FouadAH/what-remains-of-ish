using System;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => m_ID = Guid.NewGuid().ToString();

    public bool open = false;
    string key;
    int stateInt;
    public Lever lever;

    public UnityEvent OnDoorOpen;
    Animator anim;
    // Start is called before the first frame update
    void Start()    
    {
        anim = GetComponent<Animator>();
        if (lever != null)
        {
            lever.OnToggle += Lever_OnToggle;
        }

        key = "Door_" + ID;
        open = (PlayerPrefs.GetInt(key) == 1) ? true : false;
        
        SetStateInitial(open);
    }

    private void Lever_OnToggle(bool isOpen)
    {
        SetState(isOpen);
    }

    void SetStateInitial(bool open)
    {
        this.open = open;
        stateInt = (open) ? 1 : 0;
        PlayerPrefs.SetInt(key, stateInt);
        PlayerPrefs.Save();

        anim.SetBool("isOpen", open);
    }

    public void SetState(bool open)
	{
		this.open = open;
        stateInt = (open) ? 1 : 0;
        PlayerPrefs.SetInt(key, stateInt);
        PlayerPrefs.Save();

        anim.SetBool("isOpen", open);
        OnDoorOpen.Invoke();
    }
}
