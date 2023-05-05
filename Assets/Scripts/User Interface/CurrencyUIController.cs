using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUIController : MonoBehaviour
{
    [Header("HUD UI elements")]
    public TMP_Text currencyText;

    [Header("Data")]
    public PlayerDataSO playerData;

    void Update()
    {
        UpdateCurrencyUI();
    }

    void UpdateCurrencyUI()
    {
        currencyText.SetText(playerData.playerCurrency.Value.ToString());
    }

    //IEnumerator CurrencyCounter()
    //{
    //    float currencyDiff = playerData.playerCurrency.Value - previousCurrency;

    //    while (currencyDiff > 0)
    //    {
    //        yield return new WaitForSeconds(0.05f);
    //        currencyDiff = playerData.playerCurrency.Value - previousCurrency;
    //        previousCurrency++;
    //    }

    //    previousCurrency = playerData.playerCurrency.Value;
    //    currencyText.SetText(playerData.playerCurrency.Value.ToString());

    //    yield return new WaitForEndOfFrame();
    //}
}
