using UnityBase.UI.Config.SO;
using UnityBase.UI.Dynamic;
using UnityBase.UI.ViewCore;
using UnityEngine;
using VContainer;

public class MoveInOutUI : DynamicUI
{
    [SerializeField] private MoveInOutViewConfigSO _moveInOutViewConfig;
    
    private IMoveInOutView _moveInOutView;
    
    public override void Construct(IViewBehaviourFactory viewBehaviourFactory, IObjectResolver resolver)
    {
        _moveInOutView = _viewBehaviourFactory.CreateViewLocalAnimation<MoveInOutView>(resolver)
                                              .Initialize(_rectTransform)
                                              .Configure(_moveInOutViewConfig);
    }

    public override void OpenView() => _moveInOutView.MoveIn();
    public override void CloseView() => _moveInOutView.MoveOut();
    public override void OpenViewInstantly() => _moveInOutView.MoveInInstantly();
    public override void CloseViewInstantly() => _moveInOutView.MoveOutInstantly();
    protected override void OnDestroy() => _moveInOutView.Dispose();
}