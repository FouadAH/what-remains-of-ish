using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTriggerTrap : MonoBehaviour
{
    public float initialPause = 0;

    public float startDuration = 1f;
    public float endDuration = 1f;

    public float pauseTimeBegin = 1;
    public float pauseTimeEnd = 1;

    public float moveDistance = 5;

    float endValue;
    float startValue;

    Sequence mySequence;

    private void Start()
    {
        startValue = transform.position.y;
        endValue = transform.position.y - moveDistance;
        
    }

    void InitSequence()
    {
        if (transform != null)
        {
            mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveY(endValue, startDuration));
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValue, endDuration));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveY(endValue, startDuration));
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValue, endDuration));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValue, endDuration));
        }
    }
}
