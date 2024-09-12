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
            LoadSfx();
        }

        public void ChangeMusicVolume()
        {
            float musicVolume = musicVolumeSlider.value;
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            ApplyMusicVolume(musicVolume);
        }

        public void ChangeSfxVolume()
        {
            float sfxVolume = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            ApplySfxVolume(sfxVolume);
        }

        private void LoadVolume()
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            musicVolumeSlider.value = musicVolume;
            ApplyMusicVolume(musicVolume);
        }

        private void LoadSfx()
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxVolumeSlider.value = sfxVolume;
            ApplySfxVolume(sfxVolume);
        }

        private void ApplyMusicVolume(float volume)
        {
            BGMusic_selector.Instance.SetVolume(volume);
        }

        private void ApplySfxVolume(float volume)
        {
            SFX_selector.Instance.SetVolume(volume);
        }
    }
}