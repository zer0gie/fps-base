using System;
using UnityEngine;
using Zenject;
public class AudioManager : MonoBehaviour, IAudioManager, IDisposable, IInitializable
{
    private AudioSource _soundSource;
    private AudioSource _playerSource;
    private AudioSource _weaponSource;
    private AudioSource _backgroundSource;

    public void Initialize()
    {
        _soundSource = gameObject.AddComponent<AudioSource>();
        _playerSource = gameObject.AddComponent<AudioSource>();
        _backgroundSource = gameObject.AddComponent<AudioSource>();
        _weaponSource = gameObject.AddComponent<AudioSource>();
    }
    public void PlayBackground(AudioClip clip, float volume = 1, bool loop = true)
    {
        if (clip == null) return;
        _backgroundSource.clip = clip;
        _backgroundSource.volume = volume;
        _backgroundSource.loop = loop;
        _backgroundSource.Play();
    }

    public void PlayPlayerSound(AudioClip clip, float volume = 1)
    {
        if (clip == null) return;
        _playerSource.clip = clip;
        _playerSource.volume = volume;
        _playerSource.Play();
    }
    public void PlayWeaponSound(AudioClip clip, float volume = 1)
    {
        if (clip == null) return;
        _weaponSource.clip = clip;
        _weaponSource.volume = volume;
        _weaponSource.Play();
    }
    public void StopWeaponSounds()
    {
        if (_weaponSource != null)
        {
            _weaponSource.Stop();
        }
    }
    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if (clip == null) return;
        _soundSource.clip = clip;
        _soundSource.volume = volume;
        _soundSource.Play();
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void StopBackground()
    {
        if (_backgroundSource != null)
        {
            _backgroundSource.Stop();
        }
    }

    public void Dispose()
    {
        if (_soundSource != null)
        {
            _soundSource.Stop();
            Destroy(_soundSource);
        }
        if (_playerSource != null)
        {
            _playerSource.Stop();
            Destroy(_playerSource);
        }
        if (_backgroundSource != null)
        {
            _backgroundSource.Stop();
            Destroy(_backgroundSource);
        }
        if (_weaponSource != null)
        {
            _weaponSource.Stop();
            Destroy(_weaponSource);
        }
    }
}
