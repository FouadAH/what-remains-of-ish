using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [SerializeField] private int levelToLoad;
    [SerializeField] private int levelToUnload;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Loading Level:" + levelToLoad);
            GameManager.instance.LoadScene(levelToUnload, levelToLoad);
        }
    }
}
