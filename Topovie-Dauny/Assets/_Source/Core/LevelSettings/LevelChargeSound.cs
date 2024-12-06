using FMOD.Studio;
using SoundSystem;

namespace Core.LevelSettings
{
    public class LevelChargeSound
    {
        private readonly AudioManager _audioManager;
        private readonly EventInstanceWrapper _soundWrapper;
        private readonly StatesChanger _statesChanger;
        public LevelChargeSound(AudioManager audioManager, StatesChanger statesChanger)
        {
            _statesChanger = statesChanger;
            _audioManager = audioManager;
            _soundWrapper = new EventInstanceWrapper(_audioManager.FMODEvents.ChargingSound);
            _statesChanger = statesChanger;
            _statesChanger.OnStateChanged += PlaySoundOnStateChange;
        }
        public void PlayChargeSound()
        {
            _audioManager.StartPlayingSound(_soundWrapper);
        }
        public void StopChargeSound()
        {
            _audioManager.StopPlayingSound(_soundWrapper, STOP_MODE.ALLOWFADEOUT);
        }
        public void CleanUp()
        {
            _statesChanger.OnStateChanged -= PlaySoundOnStateChange;
        }
        private void PlaySoundOnStateChange(GameStates state)
        {
            if (state == GameStates.Fight)
            {
                _audioManager.PlayOneShot(_audioManager.FMODEvents.AttackStartedSound);
            }
            else if (state == GameStates.PortalCharged)
            {
                _audioManager.PlayOneShot(_audioManager.FMODEvents.PortalEnabledSound);
            }
        }
    }
}