using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            GameManager.instance.lastCheckpointPos = this.transform.position;
            GameManager.instance.lastCheckpointLevelIndex = SceneManager.GetActiveScene().buildIndex;  
        }
    }
}
