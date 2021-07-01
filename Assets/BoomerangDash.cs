using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangDash : MonoBehaviour
{
    public bool isDashingBoomerang;
    public bool boomerangDash;
    Vector2 boomerangPos;

    [SerializeField] private ParticleSystem afterImage;

    public ParticleSystem AfterImage { get => afterImage; set => afterImage = value; }

    void Start()
    {
        afterImage.Pause();
    }

    public void OnBoomerangDashInput(Transform transformToMove, ref Vector2 velocity, Boomerang boomerang)
    {
        StartCoroutine(TeleportEffet(0.1f));

        boomerangDash = true;
        isDashingBoomerang = true;
        boomerang.dashActive = true;

        boomerangPos = boomerang.transform.position;
        boomerang.StopBoomerang();

        Vector2 dir = (boomerangPos - (Vector2)transformToMove.position).normalized;
        boostDir = dir;

        transformToMove.position = boomerangPos;
        StartCoroutine(BoomerangDashBoost(0.1f));
        velocity.y = 0;
    }

    public IEnumerator TeleportEffet(float timer)
    {
        afterImage.Play();
        yield return new WaitForSeconds(timer);
        afterImage.Stop();
    }

    public bool doBoost = false;
    public Vector2 boostDir = Vector2.zero;

    public IEnumerator BoomerangDashBoost(float timer)
    {
        doBoost = true;
        yield return new WaitForSeconds(timer);
        doBoost = false;
    }

}
