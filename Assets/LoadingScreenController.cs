using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public GameEvent FadeOutDoneEvent;
    public GameEvent FadeInDoneEvent;

    public void PlayLoadingScreen()
    {
        GameManager.instance.isLoading = true;
        canvasGroup.DOFade(1, 1f).OnComplete(() =>
        {
            OnFadeOutDone();
        });
    }

    public void StopLoadingScreen()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        GameManager.instance.isLoading = false;
        yield return new WaitForSeconds(0.2f);
        canvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            OnFadeInDone();
        });
    }

    void OnFadeInDone()
    {
        FadeInDoneEvent.Raise();
        GameManager.instance.isLoading = false;
    }

    void OnFadeOutDone()
    {
        FadeOutDoneEvent.Raise();
    }


}
