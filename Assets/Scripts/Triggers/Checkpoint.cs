using UnityEngine.SceneManagement;
using UnityEngine;

public class Checkpoint: MonoBehaviour {

    [SerializeField]private Animator prompt;
    private int levelIndex;

    private void Start()
    {
        levelIndex = gameObject.scene.buildIndex;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopIn");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            GameManager.instance.lastCheckpointLevelIndex = levelIndex;
            GameManager.instance.lastCheckpointPos = transform.position;
            GameManager.instance.SaveGame();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopOut");
        }
    }
}
