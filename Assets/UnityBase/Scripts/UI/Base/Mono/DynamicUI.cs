using Sirenix.OdinInspector;
using UnityBase.UI.ViewCore;
using UnityEngine;
using VContainer;

namespace UnityBase.UI.Dynamic
{
    public abstract class DynamicUI : MonoBehaviour, IDynamicUI
    {
        [ReadOnly] [SerializeField] protected RectTransform _rectTransform;

        protected IViewBehaviourFactory _viewBehaviourFactory;
        
#if UNITY_EDITOR
        protected void OnValidate() => _rectTransform = GetComponent<RectTransform>();
#endif
        protected void Awake() => _rectTransform ??= GetComponent<RectTransform>();

        [Inject]
        public void Initialize(IViewBehaviourFactory viewBehaviourFactory, IObjectResolver resolver)
        {
            _viewBehaviourFactory = viewBehaviourFactory;
            Construct(_viewBehaviourFactory, resolver);
        }
        public abstract void Construct(IViewBehaviourFactory viewBehaviourFactory, IObjectResolver resolver);
        public abstract void OpenView();
        public abstract void CloseView();
        public abstract void OpenViewInstantly();
        public abstract void CloseViewInstantly();
        protected abstract void OnDestroy();
    }
    public interface IDynamicUI
    {
        public void OpenView();
        public void CloseView();
        public void OpenViewInstantly();
        public void CloseViewInstantly();
    }
}

