using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class SavePoint: MonoBehaviour {

    [SerializeField] private Animator prompt;
    private int levelIndex;
    private string levelPath;

    public Sprite litSprite;
    public UnityEngine.Rendering.Universal.Light2D pointLight;
    public Transform playerSpawnPoint;

    public Transform playerTargetRight;
    public Transform playerTargetLeft;

    [Header("VFX")]
    public GameObject unlitVFXParent;
    public GameObject litVFXParent;

    [Header("Events")]
    public GameEvent OnRestEvent;
    public GameEvent EnablePlayerInputEvent;
    public GameEvent DisablePlayerInputEvent;

    [Header("Data")]
    public PlayerRuntimeDataSO playerRuntimeData;

    bool isLit = false;
    bool playerIsInTrigger;

    SpriteRenderer spriteRenderer;
    GameObject playerObj;
    Player player;

    private void Start()
    {
        levelIndex = gameObject.scene.buildIndex;
        levelPath = gameObject.scene.path;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayerInput_OnInteract()
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
            playerObj = collision.gameObject;
            player = playerObj.GetComponent<Player>();
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

        //StartCoroutine(MovePlayerToTarget());

        if (!isLit)
        {
            isLit = true;
            pointLight.enabled = true;
            spriteRenderer.sprite = litSprite;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Save Point", GetComponent<Transform>().position);

            foreach (ParticleSystem particleSystem in unlitVFXParent.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Play();
            }

            foreach (ParticleSystem particleSystem in litVFXParent.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Play();
            }
        }

        PlayerDataSO playerData = player.playerData;
        if (playerData != null)
        {
            playerData.playerPosition.X = playerSpawnPoint.position.x;
            playerData.playerPosition.Y = playerSpawnPoint.position.y;

            playerData.lastSavepointLevelIndex.Value = levelIndex;
            playerData.lastSavepointLevelPath = levelPath;

            playerData.lastSavepointPos.X = playerSpawnPoint.position.x;
            playerData.lastSavepointPos.Y = playerSpawnPoint.position.y;

            playerData.lastCheckpointLevelIndex.Value = levelIndex;
            playerData.lastCheckpointLevelPath = levelPath;

            playerData.lastCheckpointPos.X = playerSpawnPoint.position.x;
            playerData.lastCheckpointPos.Y = playerSpawnPoint.position.y;

            for (int i = 0; i < playerData.playerHealingPodAmount.Value; i++)
            {
                playerData.playerHealingPodFillAmounts[i] = 100;
            }
        }

        float missingHealth = player.playerData.playerMaxHealth.Value - player.playerData.playerHealth.Value;
        player.RestoreHP((int)missingHealth);

        OnRestEvent.Raise();

        SaveManager.instance.RestPointSave();
    }

    //float marginOfError = 1f;
    //IEnumerator MovePlayerToTarget()
    //{
    //    DisablePlayerInputEvent.Raise();

    //    Vector2 playerPos = playerRuntimeData.playerPosition;

    //    float desiredXPosition = (Vector2.Distance(playerPos, playerTargetRight.transform.position) > Vector2.Distance(playerPos, playerTargetLeft.transform.position))
    //        ? playerTargetLeft.transform.position.x : playerTargetRight.transform.position.x;

    //    int directionX = (desiredXPosition > playerPos.x) ? 1 : -1;

    //    while (true)
    //    {
    //        playerInput.directionalInput.x = directionX;
    //        playerPos = playerRuntimeData.playerPosition;

    //        if (FastApproximately(desiredXPosition, playerPos.x, marginOfError))
    //        {
    //            playerInput.directionalInput.x = 0;
    //            break;
    //        }
    //        yield return null;
    //    }

    //    EnablePlayerInputEvent.Raise();
    //}

    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

}
