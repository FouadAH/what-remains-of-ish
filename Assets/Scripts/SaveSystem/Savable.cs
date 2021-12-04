using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Savable : MonoBehaviour, ISaveable
{
    protected GuidComponent m_ID;
    public string ID { get => m_ID.GetGuid().ToString(); } 

    public virtual void Awake()
    {
        m_ID = GetComponent<GuidComponent>();

        if(SaveManager.instance != null)
            SaveManager.instance.RegisterSaveable(this, ID);
    }

    public virtual void Start()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.RegisterSaveable(this, ID);
    }

    public virtual void OnDestroy()
    {
        SaveManager.instance.UnegisterSaveable(this, ID);
    }

    /// <summary>
    /// Saving data from objects to file.
    /// </summary>
    public abstract string SaveData();

    /// <summary>
    /// Prepares the default data for each objects.
    /// </summary>
    public abstract void LoadDefaultData();

    /// <summary>
    /// Loading data from a file to the object. If data can differ between versions, make sure to implement a way to migrate data using the version string.
    /// </summary>
    public abstract void LoadData(string data, string version);
}

public interface ISaveable
{
    string SaveData();
    void LoadDefaultData();
    void LoadData(string data, string version);
}