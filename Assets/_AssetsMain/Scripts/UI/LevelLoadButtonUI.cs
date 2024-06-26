using DG.Tweening;
using UnityBase.UI.ButtonCore;
using VContainer;

public class LevelLoadButtonUI : ButtonUI
{
    protected override void Initialize(IButtonBehaviourFactory buttonBehaviourFactory, IObjectResolver resolver)
    {
        _buttonAction = buttonBehaviourFactory.CreateButtonAction<LevelLoadAction>(this, resolver)
                                              .Configure(SceneType.Gameplay, true);
        
        _buttonAnimation = buttonBehaviourFactory.CreateButtonAnimation<ButtonClickAnim>(this, resolver)
                                                 .Configure(1.05f, 0.1f, Ease.InOutQuad);
    }
}
