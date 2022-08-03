using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossArena : MonoBehaviour, ISaveable
{
    public List<Door> arenaDoors;
    public Entity bossEntity;

    public CinemachineVirtualCamera bossAreaCamera;
    public Canvas bossHealthCanvas;
    bool hasBeenActivated;  
    public BossArenaData arenaData;
    public PlayableDirector bossIntroCutscene;
    [FMODUnity.EventRef] public string bossThemeMusic;
    string levelTheme;


    public struct BossArenaData
    {
        public bool isFinished;
    }

    private void Start()
    {
        foreach (Door door in arenaDoors)
        {
            door.SetStateInitial(true);
        }
    }

    private void Update()
    {
        if (!hasBeenActivated || arenaData.isFinished)
            return;

        if (bossEntity.isDead)
        {
            OnBossDead();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenActivated || arenaData.isFinished)
            return;

        if (collision.GetComponent<Player>())
        {
            StartBossFight();
        }
    }

    public void StartBossFight()
    {
        if (bossThemeMusic.Equals("") == false)
        {
            levelTheme = AudioManager.instance.currentTheme;
            AudioManager.instance.SwitchLevelMusicEvent(bossThemeMusic);
        }
        
        AudioManager.instance.StopAreaAmbianceWithFade();
        AudioManager.instance.SetIntensity(50);

        hasBeenActivated = true;

        foreach (Door door in arenaDoors)
        {
            door.SetState(false);
        }

        if (bossIntroCutscene != null)
        {
            bossIntroCutscene.Play();
        }
        else
        {
            bossEntity.gameObject.SetActive(true);
        }

        bossAreaCamera.gameObject.SetActive(true);
        bossAreaCamera.Follow = GameManager.instance.player.transform;
        bossAreaCamera.LookAt = GameManager.instance.player.transform;
        //bossHealthCanvas.enabled = true;
    }

    public void OnEnterPhase2()
    {
        AudioManager.instance.SetIntensity(100);
    }

    public void OnBossDead()
    {
        if (bossThemeMusic.Equals("") == false)
        {
            AudioManager.instance.SwitchLevelMusicEvent(levelTheme);
        }

        AudioManager.instance.StopSFXWithFade();
        AudioManager.instance.SetIntensity(0);
        AudioManager.instance.PlayAreaAmbiance();

        arenaData.isFinished = true;

        foreach (Door door in arenaDoors)
        {
            door.SetState(true);
        }

        bossAreaCamera.enabled = false;
        bossHealthCanvas.enabled = false;
    }

    public string SaveData()
    {
        return JsonUtility.ToJson(arenaData);
    }

    public void LoadDefaultData()
    {
        arenaData.isFinished = false;
    }

    public void LoadData(string data, string version)
    {
        arenaData = JsonUtility.FromJson<BossArenaData>(data);
        Debug.Log("loading boss arena data: " + data);
    }
}


