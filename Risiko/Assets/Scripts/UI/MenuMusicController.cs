using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    private AudioSource audioSource;
    private static MenuMusicController instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play(); // Avvia la musica quando viene caricato il menu
    }

    public void StopMusic()
    {
        audioSource.Stop(); // Ferma la musica
    }
}