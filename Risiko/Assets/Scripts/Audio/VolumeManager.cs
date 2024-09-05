using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeManager : MonoBehaviour
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        public void Start()
        {
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1);
            }
            if (!PlayerPrefs.HasKey("SFXVolume"))
            {
                PlayerPrefs.SetFloat("SFXVolume", 1);
            }
            LoadVolume();
            LoadSFX();
        }

        public void ChangeMusicVolume()
        {
            float musicVolume = musicVolumeSlider.value;
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            ApplyMusicVolume(musicVolume);
        }

        public void ChangeSFXVolume()
        {
            float sfxVolume = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            ApplySFXVolume(sfxVolume);
        }

        private void LoadVolume()
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            musicVolumeSlider.value = musicVolume;
            ApplyMusicVolume(musicVolume);
        }

        private void LoadSFX()
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxVolumeSlider.value = sfxVolume;
            ApplySFXVolume(sfxVolume);
        }

        private void ApplyMusicVolume(float volume)
        {
            // Assuming you have a reference to your music AudioSource
            BGMusic_selector.Instance.SetVolume(volume);
        }

        private void ApplySFXVolume(float volume)
        {
            // Assuming you have a reference to your SFX AudioSource
            SFX_selector.Instance.SetVolume(volume);
        }
    }
}
/*using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeManager : MonoBehaviour
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        
        public void Start()
        {
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1);
                LoadVolume();
            }
            else
            {
                LoadVolume();
            }
            
            if (!PlayerPrefs.HasKey("SFXVolume"))
            {
                PlayerPrefs.SetFloat("SFXVolume", 1);
                LoadSFX();
            }
            else
            {
                LoadSFX();
            }
        }
    
        public void ChangeVolume()
        {
            AudioListener.volume = volumeSlider.value; // Cambia volume globale
            Save();
        }

        private void LoadVolume()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }
        
        private void LoadSFX()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            Debug.Log("Salvato libello volumen: " + PlayerPrefs.GetFloat("musicVolume"));
        }
    }
}*/
