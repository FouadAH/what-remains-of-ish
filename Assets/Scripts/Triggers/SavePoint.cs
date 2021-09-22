using UnityEngine.SceneManagement;
using UnityEngine;

public class SavePoint: MonoBehaviour {

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
            Rest();
        }
    }

    private void Rest()
    {
        GameManager.instance.lastSavepointLevelIndex = levelIndex;
        GameManager.instance.lastSavepointPos = transform.position;

        GameManager.instance.lastCheckpointLevelIndex = levelIndex;
        GameManager.instance.lastCheckpointPos = transform.position;

        float missingHealth = GameManager.instance.maxHealth - GameManager.instance.health;
        GameManager.instance.health = GameManager.instance.maxHealth;
        UI_HUD.instance.OnResetHP(missingHealth);

        GameManager.instance.SaveGame();

        UI_HUD.instance.SetDebugText("Player health restored, checkpoint set and game saved.");
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopOut");
        }
    }
}
