using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    FMOD.Studio.EventInstance Event;
    FMOD.Studio.EventInstance attackEvent;

    void PlayEvent(string path)
    {
        foreach (FMODUnity.StudioListener item in FMODUnity.RuntimeManager.Listeners)
        {
            Debug.Log(item);

        }
        FMODUnity.RuntimeManager.PlayOneShotAttached(path, gameObject);
    }

    public void PlayEventOnce(string path)
    {
        if (!IsPlaying(Event))
        {
            Event = FMODUnity.RuntimeManager.CreateInstance(path);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Event, transform, GetComponent<Rigidbody2D>());
            Event.start();
        }
    }

    void PlayAttackEvent(string path)
    {
        attackEvent = FMODUnity.RuntimeManager.CreateInstance(path);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(attackEvent, transform, GetComponent<Rigidbody2D>());
        attackEvent.start();
        attackEvent.release();
    }

    public void StopPlayingEvent()
    {
        FMODUnity.RuntimeManager.DetachInstanceFromGameObject(Event);
        Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Event.release();
    }

    void StopAttackEvent()
    {
        attackEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    private void OnDestroy()
    {
        FMODUnity.RuntimeManager.DetachInstanceFromGameObject(Event);
        Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Event.release();
    }
}