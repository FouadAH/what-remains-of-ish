using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;

    // Start is called before the first frame update
    void Start()
    {
		SetState(open);
    }

    public void SetState(bool open)
	{
		this.open = open;
		GetComponent<Animator>().SetBool("isOpen", open);
	}
}
