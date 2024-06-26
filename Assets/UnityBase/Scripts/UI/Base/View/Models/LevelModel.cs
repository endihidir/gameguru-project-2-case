using TMPro;

namespace UnityBase.UI.ViewCore
{
    public class LevelModel : ILevelModel
    {
        private TextMeshProUGUI _levelTxt;
        
        public ILevelModel Initialize(TextMeshProUGUI levelTxt)
        {
            _levelTxt = levelTxt;
            
            return this;
        }

        public void UpdateLevelView(int value) => _levelTxt.text = "LEVEL " + value;
        public void Dispose() {}
    }
    
    public interface ILevelModel : IViewModel
    {
        public ILevelModel Initialize(TextMeshProUGUI levelTxt);
        public void UpdateLevelView(int value);
    }
}