using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SoundSystem
{
    public class VolumeSlider: MonoBehaviour
    {
        private enum VolumeType
        {
            Master,
            Music,
            Sfx
        }

        [Header("Type")] 
        [SerializeField] private VolumeType volumeType;

        private Slider _volumeSlider;
        private AudioManager _audioManager;

        [Inject]
        public void Construct(AudioManager soundManager)
        {
            _audioManager = soundManager;
        }
        private void Start()
        {
            _volumeSlider = GetComponent<Slider>();
            _volumeSlider.onValueChanged.AddListener(_ => OnSliderValueChanged());
            
            switch (volumeType)
            {
                case VolumeType.Master:
                    _volumeSlider.value = _audioManager.MasterVolume;
                    break;
                case VolumeType.Sfx:
                    _volumeSlider.value = _audioManager.SfxVolume;
                    break;
                case VolumeType.Music:
                    _volumeSlider.value = _audioManager.MusicVolume;
                    break;
                default:
                    throw new Exception($"Volume type not supported: {volumeType}");
            }
        }
        private void OnSliderValueChanged()
        {
            switch (volumeType)
            {
                case VolumeType.Master:
                    _audioManager.MasterVolume = _volumeSlider.value;
                    break;
                case VolumeType.Sfx:
                    _audioManager.SfxVolume = _volumeSlider.value;
                    break;
                case VolumeType.Music:
                    _audioManager.MusicVolume = _volumeSlider.value;
                    break;
                default:
                    throw new Exception($"Volume is not supported {volumeType}");
            }
        }
    }
}