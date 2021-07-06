using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrusherTrap : MonoBehaviour
{
    public float initialPause = 0;

    public float startDuration = 1f;
    public float endDuration = 1f;

    public float pauseTimeBegin = 1;
    public float pauseTimeEnd = 1;

    public float moveDistance = 5;

    float endValue;
    float startValue;

    private void Start()
    {
        startValue = transform.position.y;
        endValue = transform.position.y - moveDistance;
        StartCoroutine(CrushSequence());
    }

    void Crush()
    {
        if (transform != null)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveY(endValue, startDuration));
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValue, endDuration));
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        //mySequence.PrependInterval(initialPause);
    }

    IEnumerator CrushSequence()
    {
        yield return new WaitForSeconds(initialPause);
        Crush();
    }


}
