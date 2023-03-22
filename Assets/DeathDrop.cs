using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class DeathDrop : Savable
{
    DeathDropData dropData;
    bool isInTrigger = false;

    public UnityAction<int> pickUpEvent;

    public void InitializeDeathDrop(int amount, Vector2 position)
    {
        dropData = new DeathDropData
        {
            coinAmount = amount,
            dropPosition = position
        };
    }

    private void Update()
    {
        if (!isInTrigger)
        {
            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    public void Interact()
    {
        pickUpEvent?.Invoke(dropData.coinAmount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInTrigger = false;
        }
    }

    public override void LoadData(string data, string version)
    {
        dropData = JsonUtility.FromJson<DeathDropData>(data);
    }

    public override void LoadDefaultData()
    {
        InitializeDeathDrop(0, Vector2.zero);
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(dropData);
    }
}

[Serializable]
public struct DeathDropData
{
    public int coinAmount;
    public Vector2 dropPosition;
}
