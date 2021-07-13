using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
    private float oldPosition;
    void Start()
    {
        oldPosition = transform.position.x;
    }
    void FixedUpdate()
    {
        if (transform.position.x != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }
            oldPosition = transform.position.x;
        }
    }

    //public float pixPerUnit = 10;
    //void LateUpdate()
    //{
    //    transform.position = new Vector3(
    //        Mathf.Round(transform.position.x * pixPerUnit) / pixPerUnit,
    //        Mathf.Round(transform.position.y * pixPerUnit) / pixPerUnit,
    //        transform.position.z);
    //}
}