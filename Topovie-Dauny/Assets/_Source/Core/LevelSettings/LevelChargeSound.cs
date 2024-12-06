using FMOD.Studio;
using SoundSystem;

namespace Core.LevelSettings
{
    public class LevelChargeSound
    {
        private readonly AudioManager _audioManager;
        private readonly EventInstanceWrapper _soundWrapper;
        public LevelChargeSound(AudioManager audioManager)
        {
            _audioManager = audioManager;
            _soundWrapper = new EventInstanceWrapper(_audioManager.FMODEvents.ChargingSound);
        }
        public void PlayChargeSound()
        {
            _audioManager.StartPlayingSound(_soundWrapper);
        }
        public void StopChargeSound()
        {
            _audioManager.StopPlayingSound(_soundWrapper, STOP_MODE.ALLOWFADEOUT);
        }
    }
}