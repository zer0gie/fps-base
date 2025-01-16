using UnityEngine;

public interface IAudioManager
{
    void PlayPlayerSound(AudioClip clip, float volume = 1.0f);
    void PlayWeaponSound(AudioClip clip, float volume = 1.0f);
    void StopWeaponSounds();
    void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f);
    void PlayBackground(AudioClip clip, float volume = 1.0f, bool loop = true);
    void StopBackground(); 
}
