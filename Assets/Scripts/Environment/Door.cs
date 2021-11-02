using System;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()    
    {
        if (lever != null)
        {
            lever.OnToggle += Lever_OnToggle;
        }
        key = "Door_" + ID;
        open = (PlayerPrefs.GetInt(key) == 1) ? true : false;
		SetState(open);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Door", GetComponent<Transform>().position);
    }

    private void Lever_OnToggle(bool isOpen)
    {
        SetState(isOpen);
    }

    public void SetState(bool open)
	{
		this.open = open;
        stateInt = (open) ? 1 : 0;
        PlayerPrefs.SetInt(key, stateInt);
        PlayerPrefs.Save();

        GetComponent<Animator>().SetBool("isOpen", open);
	}
}
