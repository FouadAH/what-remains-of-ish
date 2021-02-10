using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] LauncherSettings launcherSettings;
    private ILauncher launcher;
    private GameManager gm;

    public bool heatable = false;

    [SerializeField] private float heat;
    [SerializeField] private float heatPerShot = 0f;
    [SerializeField] private float heatSink = 0.25f;
    [SerializeField] private float heatMax = 50f;

    void Start()
    {
        launcher = GetComponent<ILauncher>();
        
        if (launcherSettings.UsedByAi)
        {
            GetComponentInParent<FiringAI>().OnFire += Fire;
        }
        else
        {
            gm = GameManager.instance;
            gm.gunHeat = heat;
            gm.maxGunHeat = heatMax;
            gm.player.GetComponent<Player_Input>().OnFire += Fire;
        }   
        
    }

    private void Update()
    {
        heat = Mathf.Max(heat - heatSink * Time.deltaTime, 0);
        if (!launcherSettings.UsedByAi)
        {
            gm.gunHeat = heat;
        }
    }

    public void Fire()
    {
        if (!CanFire())
            return;

        if(!launcherSettings.UsedByAi)
            heat += heatPerShot;

        launcher.Launch(this);
    }

    private bool CanFire()
    {
        return heat <= heatMax;
    }
}
