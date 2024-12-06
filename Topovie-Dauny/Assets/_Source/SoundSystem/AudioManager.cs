using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SoundSystem
{
    public class AudioManager: MonoBehaviour
    {
        [BankRef] public List<string> banks;
        
        [field:Header("Volume")] 
        [field:Range(0, 1)] public float MasterVolume { get; set; } = 1;
        [field:Range(0, 1)] public float MusicVolume { get; set; } = 1;
        [field:Range(0, 1)] public float SfxVolume { get; set; } = 1;
        [field:SerializeField] public FMODEvents FMODEvents { get; private set; }

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _sfxBus;
        private List<EventInstance> _eventInstances = new();
        private List<StudioEventEmitter> _eventEmitters = new();
        private EventInstance _musicEventInstance;
        private void Awake()
        {
            LoadBanks();
        }
        private void Start()
        {
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");
            InitializeMusic(FMODEvents.MenuMusic);
        }
        private void Update()
        {
            _masterBus.setVolume(MasterVolume);
            _musicBus.setVolume(MusicVolume);
            _sfxBus.setVolume(SfxVolume);
        }
        private void OnDestroy()
        {
            CleanUp();
        }
        public void PlayOneShot(EventReference sound)
        {
            RuntimeManager.PlayOneShot(sound);
        }
        public void PlayOneShot(EventReference sound, Vector3 worldPos, Vector3 listenerPos, float maxDistance)
        {
            var distance = Vector3.Distance(worldPos, listenerPos);
            var attenuation = Mathf.Clamp01(1 - (distance / maxDistance));
            var instance = RuntimeManager.CreateInstance(sound);
            instance.set3DAttributes(worldPos.To3DAttributes());
            instance.setVolume(attenuation);
            instance.start();
            instance.release();
        }
        public void ChangeMusic(EventReference music, STOP_MODE stopMode)
        {
            _musicEventInstance.stop(stopMode);
            _musicEventInstance.release();
            InitializeMusic(music);
        }
        public void PlayMusicDuringTime(float time, EventReference music)
        {
            var instance = CreateInstance(music);
    
            var wrapper = new EventInstanceWrapper(instance);
    
            wrapper.Instance.start();

            StopMusicAfterTime(wrapper, time).Forget();
        }
        public void StartPlayingSound(EventInstanceWrapper soundWrapper)
        {
            soundWrapper.Instance.start();
        }
        public void StopPlayingSound(EventInstanceWrapper soundWrapper, STOP_MODE stopMode)
        {
            soundWrapper.Instance.stop(stopMode);
        }
        private void InitializeMusic(EventReference musicEventReference)
        {
            _musicEventInstance = CreateInstance(musicEventReference);
            _musicEventInstance.start();
        }
        private void CleanUp()
        {
            foreach (var eventInstance in _eventInstances)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        private EventInstance CreateInstance(EventReference eventReference)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }
        private async UniTask StopMusicAfterTime(EventInstanceWrapper wrapper, float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
    
            wrapper.Instance.stop(STOP_MODE.ALLOWFADEOUT);
    
            wrapper.Instance.release();
    
            _eventInstances.Remove(wrapper.Instance);
        }
        private void LoadBanks()
        {
            foreach (var b in banks)
            {
                RuntimeManager.LoadBank(b, true);
                Debug.Log("Loaded bank " + b);
            }

            RuntimeManager.CoreSystem.mixerSuspend();
            RuntimeManager.CoreSystem.mixerResume();
        }
    }
    public class EventInstanceWrapper
    {
        public EventInstance Instance { get; }
        public EventInstanceWrapper(EventInstance instance)
        {
            Instance = instance;
        }
        public EventInstanceWrapper(EventReference soundReference)
        {
            Instance = RuntimeManager.CreateInstance(soundReference);
        }
    }
}