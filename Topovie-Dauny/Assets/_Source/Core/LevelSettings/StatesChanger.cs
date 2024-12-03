using System;

namespace Core.LevelSettings
{
    public class StatesChanger
    {
        public event Action<GameStates> OnStateChanged;
        public GameStates CurrentGameState { get; private set; } = GameStates.Chill;
        public void ChangeState(GameStates state)
        {
            CurrentGameState = state;
            OnStateChanged?.Invoke(CurrentGameState);
        }
    }
}