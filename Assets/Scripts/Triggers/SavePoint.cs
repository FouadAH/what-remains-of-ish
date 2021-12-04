using UnityEngine.SceneManagement;
using UnityEngine;

public class SavePoint: MonoBehaviour {

    [SerializeField]private Animator prompt;
    private int levelIndex;
    private string levelPath;


    public Sprite litSprite;
    public Transform playerSpawnPoint;

    bool isLit = false;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        levelIndex = gameObject.scene.buildIndex;
        levelPath = gameObject.scene.path;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (!isLit)
        {
            isLit = true;
            spriteRenderer.sprite = litSprite;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Save Point", GetComponent<Transform>().position);
        }

        GameManager.instance.lastSavepointLevelPath = levelPath;
        GameManager.instance.lastSavepointLevelIndex = levelIndex;

        GameManager.instance.lastSavepointPos = playerSpawnPoint.position;

        GameManager.instance.lastCheckpointLevelIndex = levelIndex;
        GameManager.instance.lastCheckpointLevelPath = levelPath;

        GameManager.instance.lastCheckpointPos = playerSpawnPoint.position;

        float missingHealth = GameManager.instance.maxHealth - GameManager.instance.health;
        GameManager.instance.health = GameManager.instance.maxHealth;
        UI_HUD.instance.OnResetHP(missingHealth);

        SaveManager.instance.RestPointSave();
        //SaveManager.instance.SaveGame();

        UI_HUD.instance.SetDebugText("Player health and flasks restored. Checkpoint set.");
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopOut");
        }
    }
}
