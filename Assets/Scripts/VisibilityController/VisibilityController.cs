using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    Entity entity;

    private void Start()
    {
        entity = GetComponentInChildren<Entity>();
        entity.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (entity.isVisible)
        {
            if (entity.enabled == false)
            {
                entity.enabled = true;
            }
        }
    }
}
