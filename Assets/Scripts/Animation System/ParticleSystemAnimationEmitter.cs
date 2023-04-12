using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAnimationEmitter : MonoBehaviour
{
    public ParticleSystem particleEffects;

    public void PlayParticleSystem()
    {
        particleEffects.Play();
    }

    public void EndParticleSystem()
    {
        particleEffects.Stop();
    }

}
