using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MisfitStudios
{
    public class SceneLoadingController : MonoBehaviour
    {
        public static SceneLoadingController instance { get; private set; }
        [HideInInspector] public GameObject player;
        [HideInInspector] public Transform playerCurrentPosition;

        [Header("Scene Settings")]
        public PlayerConfig initalPlayerData;

        [HideInInspector] public Vector3 playerStartPosition;

        public Level currentLevel;
        public int currentSceneBuildIndex;
        public string currentScenePath;

        private string levelToUnloadPath;
        private string levelToLoadPath;

        [HideInInspector] public Vector2 lastCheckpointPos;
        [HideInInspector] public int lastCheckpointLevelIndex;
        [HideInInspector] public string lastCheckpointLevelPath;

        [HideInInspector] public Vector2 lastSavepointPos;
        [HideInInspector] public int lastSavepointLevelIndex;
        [HideInInspector] public string lastSavepointLevelPath;

        [HideInInspector] public Animator anim;
        AstarPath astarPath;
        
        [Header("Game State")]
        public bool isLoading = false;
        public bool isRespawning = false;
        public bool isPaused = false;

        [Header("Other")]
        public bool isFirstTimeResting = true;
        public bool hasOpenedMap = false;

        Vector3 newPlayerPos;

        public GameEvent loadNewSceneEvent;
        public GameEvent loadDataEvent;
        public GameEvent saveDataEvent;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            astarPath = FindObjectOfType<AstarPath>();
            anim = gameObject.GetComponent<Animator>();

            lastSavepointLevelPath = initalPlayerData.initialLevelPath;
            lastCheckpointLevelPath = initalPlayerData.initialLevelPath;
            levelToLoadPath = initalPlayerData.initialLevelPath;

            lastCheckpointPos = initalPlayerData.initialPlayerPosition;
            lastSavepointPos = initalPlayerData.initialPlayerPosition;
        }


        public void InitialSpawn()
        {
            //playerCamera = Camera.main;
            player.transform.position = playerStartPosition;
        }

        public void Respawn()
        {
            Debug.Log("Hard Respawn");
            if (!isRespawning)
            {
                isRespawning = true;
                isLoading = true;
                player.GetComponent<Player>().enabled = false;
                LoadScenePath(SceneManager.GetActiveScene().path, lastSavepointLevelPath);
            }
        }

        public void SoftRespawn()
        {
            Debug.Log("Soft Respawn");
            StartCoroutine(SoftRespawnRoutine());
        }

        private IEnumerator SoftRespawnRoutine()
        {
            anim.Play("Fade_Out");
            player.GetComponent<Player>().enabled = false;

            yield return new WaitForSecondsRealtime(1f);

            //playerCamera.transform.position = player.transform.position;
            player.transform.position = lastCheckpointPos;

            yield return new WaitForSecondsRealtime(1f);

            anim.Play("Fade_in");

            player.GetComponent<Player>().enabled = true;
            player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
        }

        private IEnumerator HardRespawnSameLevel()
        {
            anim.Play("Fade_Out");
            player.GetComponent<Player>().enabled = false;

            yield return new WaitForSecondsRealtime(1f);

            //playerCamera.transform.position = player.transform.position;
            player.transform.position = lastSavepointPos;

            yield return new WaitForSecondsRealtime(1f);

            anim.Play("Fade_in");

            player.GetComponent<Player>().enabled = true;
            player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
            isRespawning = false;
            isLoading = false;
        }


        public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath)
        {
            isLoading = true;
            currentScenePath = levelToLoadPath;

            player.GetComponent<Player>().enabled = false;

            this.levelToLoadPath = levelToLoadPath;
            this.levelToUnloadPath = levelToUnloadPath;

            StartCoroutine(LoadSceneRoutine());
        }

        private IEnumerator LoadSceneRoutine()
        {
            SaveManager.instance.SaveSceneData();

            anim.Play("Fade_Out");
            yield return new WaitForSecondsRealtime(1f);
            if (astarPath != null)
            {
                astarPath.gameObject.SetActive(false);
                astarPath.enabled = false;
            }
            SceneManager.UnloadSceneAsync(levelToUnloadPath).completed += UnloadScene_completed;
        }

        private void UnloadScene_completed(AsyncOperation obj)
        {
            if (obj.isDone)
            {
                SceneManager.LoadSceneAsync(levelToLoadPath, LoadSceneMode.Additive).completed += LoadScene_completed;

            }
        }

        private void LoadScene_completed(AsyncOperation obj)
        {
            if (obj.isDone)
            {
                if (!isRespawning)
                {
                    player.transform.position = newPlayerPos;
                }
                else
                {
                    player.transform.position = lastSavepointPos;
                    //playerCamera.transform.position = player.transform.position;
                    player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
                }


                isLoading = false;
                isRespawning = false;

                StartCoroutine(FadeIn());

                player.GetComponent<Player>().enabled = true;

                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));
                currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

                SaveManager.instance.SavePlayerData();

                loadNewSceneEvent.Raise();
                loadDataEvent.Raise();

                astarPath = FindObjectOfType<AstarPath>();
                if (astarPath != null)
                {
                    astarPath.Scan();
                }
            }
        }

        public IEnumerator LoadMainMenu()
        {
            anim.Play("Fade_Out");
            AudioManager.instance.StopAreaThemeWithFade();
            AudioManager.instance.StopAreaAmbianceWithFade();
            AudioManager.instance.StopSFXWithFade();
            AudioManager.instance.StopAllAudio();

            yield return new WaitForSeconds(1f);

            SaveManager.instance.SaveGame();
            anim.Play("Fade_in");

            SceneManager.LoadScene(0);
        }

        private void LoadMainMenuCompleted(AsyncOperation obj)
        {
        }
        private IEnumerator FadeIn()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            anim.Play("Fade_in");
        }

    }
}
