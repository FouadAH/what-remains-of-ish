using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCheckpoint : MonoBehaviour
{
    private int levelIndex;
    private void Start()
    {
        levelIndex = gameObject.scene.buildIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            GameManager.instance.lastSavepointLevelIndex = levelIndex;
            GameManager.instance.lastSavepointPos = transform.position;

            GameManager.instance.lastCheckpointLevelIndex = levelIndex;
            GameManager.instance.lastCheckpointPos = transform.position;
        }
    }

}
