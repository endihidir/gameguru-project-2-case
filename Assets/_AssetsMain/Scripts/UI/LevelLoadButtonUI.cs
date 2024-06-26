using DG.Tweening;
using UnityBase.UI.ButtonCore;
using UnityEngine;
using VContainer;

public class LevelLoadButtonUI : ButtonUI
{
    [SerializeField] private SceneType _sceneType;
    
    [SerializeField] private bool _useLoadingScene;
    
    protected override void Initialize(IButtonBehaviourFactory buttonBehaviourFactory, IObjectResolver resolver)
    {
        _buttonAction = buttonBehaviourFactory.CreateButtonAction<LevelLoadAction>(this, resolver)
                                              .Configure(_sceneType, _useLoadingScene);
        
        _buttonAnimation = buttonBehaviourFactory.CreateButtonAnimation<ButtonClickAnim>(this, resolver)
                                                 .Configure(1.05f, 0.1f, Ease.InOutQuad);
    }
}
