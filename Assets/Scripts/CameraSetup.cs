using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cam =GetComponent<Camera>();
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
