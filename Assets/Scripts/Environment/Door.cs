using System;
using UnityEngine;
using UnityEngine.Events;

public class Door : Savable
{
    [System.Serializable]
    public struct Data
    {
        public bool isOpen;
    }

    [SerializeField]
    private Data doorData;
    public bool isOpenDefault;

    public Lever lever;
    public UnityEvent OnDoorOpen;

    Animator anim;

    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();

        if (lever != null)
        {
            lever.OnToggle += Lever_OnToggle;
        }
    }

    private void Lever_OnToggle(bool isOpen)
    {
        SetState(isOpen);
    }

    public void SetStateInitial(bool open)
    {
        this.doorData.isOpen = open;
        anim.SetBool("isOpen", open);
    }

    public void SetState(bool open)
	{
		this.doorData.isOpen = open;
        anim.SetBool("isOpen", open);
        OnDoorOpen.Invoke();
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(doorData);
    }

    public override void LoadDefaultData()
    {
        doorData.isOpen = isOpenDefault;
        SetStateInitial(doorData.isOpen);   
    }

    public override void LoadData(string data, string version)
    {
        doorData = JsonUtility.FromJson<Data>(data);
        SetStateInitial(doorData.isOpen);
        Debug.Log("loading door state: " + data);
    }

}
