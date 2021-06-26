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

        transformToMove.position = boomerangPos;
        velocity.y = 0;
    }

    public IEnumerator TeleportEffet(float timer)
    {
        afterImage.Play();
        yield return new WaitForSeconds(timer);
        afterImage.Stop();
    }

}
