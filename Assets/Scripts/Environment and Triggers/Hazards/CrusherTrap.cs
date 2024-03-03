using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

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

    CinemachineImpulseSource impulseListener;
    [FMODUnity.EventRef] public string crusherImpactSFX;

    public ParticleSystem impactHitEffect;
    public ParticleSystem impactDustEffect;

    SpriteRenderer spriteRenderer;

    float endValueY;
    float startValueY;

    float endValueX;
    float startValueX;

    Sequence mySequence;
    int sequenceID;
    
    public Lever controlLever;
    bool isVisible = false;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        impulseListener = GetComponent<CinemachineImpulseSource>();
        if(controlLever != null)
        {
            controlLever.OnToggle += ControlLever_OnToggle;
        }

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

    private void ControlLever_OnToggle(bool obj)
    {
        StopCrusher();
    }

    void CrushY()
    {
        if (transform != null)
        {
            mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveY(endValueY, startDuration));
            mySequence.AppendCallback(ImpactEffects);
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveY(startValueY, endDuration));
            mySequence.SetLoops(-1, LoopType.Restart);

            sequenceID = mySequence.intId;
        }
        //mySequence.PrependInterval(initialPause);
    }

    void CrushX()
    {
        if (transform != null)
        {
            mySequence = DOTween.Sequence();
            mySequence.AppendInterval(pauseTimeBegin);
            mySequence.Append(transform.DOMoveX(endValueX, startDuration)).onComplete += ImpactEffects;
            mySequence.AppendInterval(pauseTimeEnd);
            mySequence.Append(transform.DOMoveX(startValueX, endDuration));
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        //mySequence.PrependInterval(initialPause);
    }

    public void StopCrusher()
    {
        mySequence.Append(transform.DOMoveY(startValueY, endDuration));
        mySequence.Kill();
    }

    void ImpactEffects()
    {
        impactDustEffect.Play();
        impactHitEffect.Play();
        impulseListener.GenerateImpulse();
        //FMODUnity.RuntimeManager.PlayOneShot(crusherImpactSFX, gameObject.transform.position);

        if (spriteRenderer.isVisible)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(crusherImpactSFX, gameObject);
        }
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

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void OnDrawGizmos()
    {
        Collider2D collider2D = GetComponentInChildren<BoxCollider2D>();
        Gizmos.color = Color.blue;

        if (crusherType == CrusherType.VerticalCrusher)
        {
            Vector3 from = new(transform.position.x, transform.position.y - collider2D.bounds.extents.y, transform.position.z);
            Vector3 to = new(transform.position.x, transform.position.y - collider2D.bounds.extents.y - moveDistanceY, transform.position.z);
            Gizmos.DrawLine(from, to);
        }
        else
        {
            Vector3 from = new(transform.position.x - collider2D.bounds.extents.x ,transform.position.y , transform.position.z);
            Vector3 to = new(transform.position.x - collider2D.bounds.extents.x - moveDistanceX, transform.position.y, transform.position.z);
            Gizmos.DrawLine(from, to);
        }
    }

}

public enum CrusherType{
    HorizontalCrusher,
    VerticalCrusher
}
