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
            PlayerDataSO playerData = collision.gameObject.GetComponent<Player>().playerData;
            if (playerData != null)
            {
                playerData.lastSavepointLevelIndex.Value = levelIndex;
                playerData.lastSavepointPos.X = transform.position.x;
                playerData.lastSavepointPos.Y = transform.position.y;

                playerData.lastCheckpointLevelIndex.Value = levelIndex;
                playerData.lastCheckpointPos.X = transform.position.x;
                playerData.lastCheckpointPos.Y = transform.position.y;
            }
        }
    }

}
