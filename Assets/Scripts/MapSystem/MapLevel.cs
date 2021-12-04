using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevel : MonoBehaviour
{
    public Level level;
    public Image levelImage;

    private void Start()
    {
        levelImage.enabled = level.isRevealed;
    }
}
