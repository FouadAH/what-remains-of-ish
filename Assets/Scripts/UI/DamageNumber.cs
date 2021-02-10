using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Color color = new Color(1f,1f,1f,1.0f);
    public float scroll = 0.05f; 
    public float duration = 2f; 
    public float alpha;
    public TextMeshProUGUI number;

    void Start()
    {
        number.color = color;
        alpha = 1;
    }

    void Update()
    {
        if (color.a > 0)
        {
            number.transform.position += new Vector3(0, scroll * Time.deltaTime);
            number.color = color;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void Setup(int damage)
    {
        number.SetText(damage.ToString());
    }
}
