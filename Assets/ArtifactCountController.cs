using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactCountController : MonoBehaviour
{
    public IntegerVariable artifactCount;
    public TMPro.TMP_Text countText;

    public void OnEnable()
    {
        countText.text = artifactCount.Value.ToString();   
    }
}
