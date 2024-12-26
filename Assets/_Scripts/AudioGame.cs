using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGame : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _loseSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioListener.volume = PlayerPrefs.GetFloat("AudioStatus", 1);
    }

    public void ClickSound()
    {
        _audioSource.PlayOneShot(_clickSound);
    }

    public void WinSound()
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(_winSound);
    }

    public void LoseSound()
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(_loseSound);
    }
}
