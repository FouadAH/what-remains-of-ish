using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour, ISaveable
{
    public List<Door> arenaDoors;
    public Entity bossEntity;

    public CinemachineVirtualCamera bossAreaCamera;

    bool hasBeenActivated;
    public BossArenaData arenaData;

    //[FMODUnity.EventRef] public string bossThemeMusic;

    public struct BossArenaData
    {
        public bool isFinished;
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
        //if(bossThemeMusic != "")
        //    AudioManager.instance.PlayAreaTheme(bossThemeMusic);

        AudioManager.instance.SetIntensity(100);

        hasBeenActivated = true;

        foreach (Door door in arenaDoors)
        {
            door.SetState(false);
        }

        bossEntity.gameObject.SetActive(true);
        bossAreaCamera.gameObject.SetActive(true);
        bossAreaCamera.Follow = GameManager.instance.player.transform;
        bossAreaCamera.LookAt = GameManager.instance.player.transform;
    }

    public void OnBossDead()
    {
        //if (bossThemeMusic != "")
        //    AudioManager.instance.StopAreaThemeWithFade();

        AudioManager.instance.SetIntensity(0);

        arenaData.isFinished = true;

        foreach (Door door in arenaDoors)
        {
            door.SetState(true);
        }

        bossAreaCamera.enabled = false;
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

