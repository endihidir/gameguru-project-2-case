using UnityBase.Manager;

namespace UnityBase.Service
{
    public interface IGameplayManager
    {
        public GameState CurrentGameState { get; }
        public void ChangeGameState(GameState nextGameState, float transitionDuration = 0f, float startDelay = 0f);
    }
}