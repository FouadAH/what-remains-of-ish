using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossArena : Savable
{
    public List<Door> arenaDoors;
    public Entity bossEntity;
    public GameEvent bossFightStartEvent;
    public GameEvent bossFightEndedEvent;

    public CinemachineVirtualCamera bossAreaCamera;
    public Canvas bossCanvas;
    Animator bossCanvasAnimator;

    bool hasBeenActivated;  
    public BossArenaData arenaData;
    public PlayableDirector bossIntroCutscene;
    [FMODUnity.EventRef] public string bossThemeMusic;
    string levelTheme;

    Player player;

    [Serializable]
    public struct BossArenaData
    {
        public bool isFinished;
    }

    public override void Start()
    {
        base.Start();
        foreach (Door door in arenaDoors)
        {
            door.SetStateInitial(true);
        }

        bossCanvasAnimator = bossCanvas.GetComponent<Animator>();
    }

    private void Update()
    {
        if (arenaData.isFinished)
            return;

        if (!hasBeenActivated)
            return;

        if (bossEntity.isDead)
            OnBossDead();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (arenaData.isFinished)
            return;

        if (hasBeenActivated)
            return;

        if (collision.GetComponent<Player>())
        {
            player = collision.GetComponent<Player>();
            StartBossFight();
        }
    }

    public void StartBossFight()
    {
        bossFightStartEvent.Raise();

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
        bossAreaCamera.Follow = player.transform;
        bossAreaCamera.LookAt = player.transform;

        bossCanvasAnimator.SetTrigger("FightStart");
    }

    public void OnEnterPhase2()
    {
        AudioManager.instance.SetIntensity(100);
    }

    public void OnBossDead()
    {
        bossFightEndedEvent.Raise();

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
        bossCanvas.enabled = false;
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(arenaData);
    }

    public override void LoadDefaultData()
    {
        arenaData.isFinished = false;
    }

    public override void LoadData(string data, string version)
    {
        arenaData = JsonUtility.FromJson<BossArenaData>(data);
        Debug.Log("loading boss arena data: " + data);
    }
}


