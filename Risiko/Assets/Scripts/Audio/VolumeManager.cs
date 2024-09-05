using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeManager : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private Slider volumeSlider;
        public void Start()
        {
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1);
                Load();
            }
            else
            {
                Load();
            }
        }
    
        public void ChangeVolume()
        {
            AudioListener.volume = volumeSlider.value; // Cambia volume globale
            Save();
        }

        private void Load()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
            Debug.Log("Salvato libello volumen: " + PlayerPrefs.GetFloat("musicVolume"));
        }

        public void ChangeFXVolume()
        {
            
        }
    }
}
