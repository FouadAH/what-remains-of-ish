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

    // Start is called before the first frame update
    void Start()    
    {
        key = "Door " + gameObject.GetInstanceID();
        Debug.Log(key + " " + PlayerPrefs.GetInt(key, 0));
        PlayerPrefs.GetInt(key, 0);
		SetState(open);
    }

    public void SetState(bool open)
	{
		this.open = open;
        stateInt = (open) ? 1 : 0;
        PlayerPrefs.SetInt(key, stateInt);

        GetComponent<Animator>().SetBool("isOpen", open);
	}
}
