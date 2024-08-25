using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    public static MenuMusicController music;
    private AudioSource _audioSource;
    private void Awake()
    {
        if (music != null)
        {
            Destroy(gameObject);
        }
        else
        {
            music = null;
        }
        DontDestroyOnLoad(this.gameObject);
        _audioSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}