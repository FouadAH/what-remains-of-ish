using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODPlayer : MonoBehaviour
{
    void PlaySound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }

    public void PlaySoundEvent(SoundEffectSO soundEffect)
    {
        FMODUnity.RuntimeManager.PlayOneShot(soundEffect.soundEffectEvent, GetComponent<Transform>().position);
    }

}
