using Cysharp.Threading.Tasks;
using UnityBase.Manager;
using UnityBase.Service;

namespace UnityBase.UI.ButtonCore
{
    public class LevelLoadAction : ButtonAction
    {
        private readonly ISceneManager _sceneManager;
        private readonly IGameplayManager _gameplayManager;
        private SceneType _sceneType;
        private bool _useLoadingScene;
        private float _progressMultiplier;
        
        public LevelLoadAction(IButtonUI buttonUI, ISceneManager sceneManager, IGameplayManager gameplayManager) : base(buttonUI)
        {
            _sceneManager = sceneManager;
            _gameplayManager = gameplayManager;
        }

        public IButtonAction Configure(SceneType sceneType, bool useLoadingScene, float progressMultiplier = 10f)
        {
            _sceneType = sceneType;
            _useLoadingScene = useLoadingScene;
            _progressMultiplier = progressMultiplier;
            return this;
        }
        
        public override async void OnClick()
        {
            _gameplayManager.ChangeGameState(GameState.GameLoadingState, 0f);
            
            await UniTask.WaitForSeconds(0.1f);
            
            _sceneManager.LoadSceneAsync(_sceneType, _useLoadingScene, _progressMultiplier);
        }
        
        public override void OnPointerDown()
        {
            
        }

        public override void OnPointerUp()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}