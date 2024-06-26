using UnityEngine;
using VContainer;

namespace UnityBase.UI.ViewCore
{
    public interface IViewBehaviourFactory
    {
        public TModel CreateViewModel<TModel>(Component component, IObjectResolver resolver) where TModel : class, IViewModel;
        public TAnim CreateViewAnimation<TAnim>(Component component, IObjectResolver resolver) where TAnim : class, IViewAnimation;
        public TModel CreateViewLocalModel<TModel>(IObjectResolver resolver) where TModel : class, IViewModel;
        public TAnim CreateViewLocalAnimation<TAnim>(IObjectResolver resolver) where TAnim : class, IViewAnimation;
        public bool TryGetModel<TViewUI, TViewModel>(out TViewModel viewModel) where TViewUI : Component where TViewModel : class, IViewModel;
        public bool TryGetViewAnimation<TViewUI, TViewAnim>(out TViewAnim viewAnimation) where TViewUI : Component where TViewAnim : class, IViewAnimation;
    }
}