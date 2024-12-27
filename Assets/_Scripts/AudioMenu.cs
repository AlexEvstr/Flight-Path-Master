using UnityEngine;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private GameObject _off;
    [SerializeField] private GameObject _on;
    [SerializeField] private AudioClip _clickSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioListener.volume = PlayerPrefs.GetFloat("AudioStatus", 1);
        if (AudioListener.volume == 1)
        {
            AudioUnmute();
        }
        else
        {
            AudioMute();
        }
    }

    public void AudioMute()
    {
        _on.SetActive(false);
        _off.SetActive(true);
        AudioListener.volume = 0;
        PlayerPrefs.SetFloat("AudioStatus", 0);
    }

    public void AudioUnmute()
    {
        _off.SetActive(false);
        _on.SetActive(true);
        AudioListener.volume = 1;
        PlayerPrefs.SetFloat("AudioStatus", 1);
    }

    public void ClickSound()
    {
        _audioSource.PlayOneShot(_clickSound);
    }
}