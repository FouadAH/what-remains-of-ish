using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class SavePoint: MonoBehaviour {

    [SerializeField]private Animator prompt;
    private int levelIndex;
    private string levelPath;

    public Sprite litSprite;
    public UnityEngine.Rendering.Universal.Light2D pointLight;
    public Transform playerSpawnPoint;

    bool isLit = false;
    SpriteRenderer spriteRenderer;

    public Transform playerTargetRight;
    public Transform playerTargetLeft;

    bool playerIsInTrigger;

    GameObject player;
    Player_Input playerInput;
    TutorialManager tutorialManager;

    private void Start()
    {
        levelIndex = gameObject.scene.buildIndex;
        levelPath = gameObject.scene.path;
        playerInput = GameManager.instance.player.GetComponent<Player_Input>();
        playerInput.OnInteract += PlayerInput_OnInteract;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void PlayerInput_OnInteract()
    {
        if (playerIsInTrigger)
        {
            Rest();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerIsInTrigger = true;
            prompt.SetTrigger("PopIn");
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerIsInTrigger = false;
            prompt.SetTrigger("PopOut");
        }
    }
    
    private void Rest()
    {
        if (GameManager.instance.isFirstTimeResting)
        {
            GameManager.instance.isFirstTimeResting = false;
            TutorialManager.instance.DisplayTutorial(TutorialType.Resting);
        }

        player = GameManager.instance.player;
        //StartCoroutine(MovePlayerToTarget());

        if (!isLit)
        {
            isLit = true;
            pointLight.enabled = true;
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

    float marginOfError = 1f;
    IEnumerator MovePlayerToTarget()
    {
        playerInput = player.GetComponent<Player_Input>();
        playerInput.OnRestStart();

        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;

        float desiredXPosition = (Vector2.Distance(playerPos, playerTargetRight.transform.position) > Vector2.Distance(playerPos, playerTargetLeft.transform.position))
            ? playerTargetLeft.transform.position.x : playerTargetRight.transform.position.x;

        int directionX = (desiredXPosition > playerPos.x) ? 1 : -1;

        while (true)
        {
            playerInput.directionalInput.x = directionX;
            playerPos = GameManager.instance.playerCurrentPosition.position;

            if (FastApproximately(desiredXPosition, playerPos.x, marginOfError))
            {
                playerInput.directionalInput.x = 0;
                break;
            }
            yield return null;
        }

        playerInput.OnRestEnd();

    }

    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

}
