using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [SerializeField] private int levelToLoad;
    [SerializeField] private int levelToUnload;

    private int levelToLoadPath;
    private int levelToUnloadPath;

    public GameObject playerPos;
    ScenePicker scenePicker;
    private void Start()
    {
        scenePicker = GetComponent<ScenePicker>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Level Path:" + scenePicker.scenePathToLoad);
            Debug.Log(SceneManager.GetActiveScene().path);
            GameManager.instance.LoadScenePath(scenePicker.scenePathToUnload, scenePicker.scenePathToLoad, playerPos.transform.position);
            //GameManager.instance.LoadScene(levelToUnload, levelToLoad, playerPos.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerPos.transform.position, 1f);
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Vector3 boxPos = boxCollider2D.transform.position;
        Vector2 vector = new Vector2(boxPos.x + boxCollider2D.offset.x, boxPos.y + boxCollider2D.offset.y);
        Gizmos.DrawWireCube(vector, GetComponent<BoxCollider2D>().size);
    }
}


