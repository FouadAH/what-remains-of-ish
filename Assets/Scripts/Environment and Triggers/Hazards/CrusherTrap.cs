using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrusherTrap : MonoBehaviour
{
    public CrusherType crusherType = CrusherType.VerticalCrusher;
    public float initialPause = 0;

    public float startDuration = 1f;
    public float endDuration = 1f;

    public float pauseTimeBegin = 1;
    public float pauseTimeEnd = 1;

    public float moveDistanceY = 5;
    public float moveDistanceX = 5;

    float endValueY;
    float startValueY;

    float endValueX;
    float startValueX;

    private void Start()
    {
        startValueY = transform.position.y;
        endValueY = transform.position.y - moveDistanceY;

        startValueX = transform.position.x;
        endValueX = transform.position.x - moveDistanceX;

        switch (crusherType)
        {
            case CrusherType.HorizontalCrusher:
                StartCoroutine(CrushSequenceX());
                break;
            case CrusherType.VerticalCrusher:
                StartCoroutine(CrushSequence());
                break;
            default:
                StartCoroutine(CrushSequence());
                break;
        }
    }

    void CrushY()
    {
        if (transform != null)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveY(endValueY, startDuration));
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValueY, endDuration));
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        //mySequence.PrependInterval(initialPause);
    }

    void CrushX()
    {
        if (transform != null)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveX(endValueX, startDuration));
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveX(startValueX, endDuration));
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        //mySequence.PrependInterval(initialPause);
    }

    IEnumerator CrushSequence()
    {
        yield return new WaitForSeconds(initialPause);
        CrushY();
    }

    IEnumerator CrushSequenceX()
    {
        yield return new WaitForSeconds(initialPause);
        CrushX();
    }


}

public enum CrusherType{
    HorizontalCrusher,
    VerticalCrusher
}
