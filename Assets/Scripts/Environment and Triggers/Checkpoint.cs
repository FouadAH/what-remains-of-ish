using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        PlayerDataSO playerData = player.playerData;

        if (playerData != null)
        {
            playerData.lastCheckpointLevelIndex.Value = SceneManager.GetActiveScene().buildIndex;
            playerData.lastCheckpointLevelPath = SceneManager.GetActiveScene().path;
            playerData.lastCheckpointPos.X = transform.position.x;
            playerData.lastCheckpointPos.Y = transform.position.y;
        }
    }
}
