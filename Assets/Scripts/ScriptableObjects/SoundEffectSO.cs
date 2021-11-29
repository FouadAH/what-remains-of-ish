using UnityEngine;

[CreateAssetMenu(fileName = "newSoundEffect", menuName = "New SFX", order = 1)]
public class SoundEffectSO : ScriptableObject
{
    [FMODUnity.EventRef] public string soundEffectEvent;
}