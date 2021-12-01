using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBlock : MonoBehaviour
{
    public Lever lever;
    float angle;
    Vector3 currentAngle;

    void Start()
    {
        currentAngle = transform.eulerAngles;
        angle = currentAngle.z;
        lever.OnToggle += Lever_OnToggle;
    }

    private void Lever_OnToggle(bool isOpen)
    {
        angle *= -1;
    }

    void Update()
    {
        currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, angle, Time.deltaTime * 5));
        transform.eulerAngles = currentAngle;
    }
}
