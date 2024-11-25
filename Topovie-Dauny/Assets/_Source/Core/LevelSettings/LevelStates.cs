using System;

namespace Core.LevelSettings
{
    public class LevelStates
    {
        public event Action<GameStates> OnStateChanged;
        private GameStates _currentGameState = GameStates.Chill;
    }
}