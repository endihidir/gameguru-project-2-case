using System;
using UnityBase.EventBus;
using UnityBase.GameDataHolder;
using UnityBase.Manager.Data;
using UnityBase.Managers.SO;
using UnityBase.Service;
using UnityBase.UI.ViewCore;
using UnityEngine;

namespace UnityBase.Manager
{
    public class LevelManager : ILevelManager, IAppBootService
    {
        public static Func<int, bool> OnSelectChapter;
        public static Func<int, bool> OnSelectLevel;

        private const string LAST_SELECTED_CHAPTER_KEY = "LastSelectedChapterKey";
        private const string LAST_SELECTED_LEVEL_KEY = "LastSelectedLevelKey";
        private const string LAST_UNLOCKED_CHAPTER_KEY = "LastUnlockedChapterKey";
        private const string LAST_UNLOCKED_LEVEL_KEY = "LastUnlockedLevelKey";
        private const string LEVEL_TEXT_KEY = "LevelTextKey";

        private readonly IViewBehaviourFactory _viewBehaviourFactory;

        #region VARIABLES

        private ChapterSO[] _chapterData;

        private int _defaultUnlockedChapterIndex = 0;

        private int _defaultUnlockedLevelIndex = 0;

        private EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>();

        #endregion

        #region PROPERTIES

        private bool IsThereMoreThanOneChapter => _chapterData.Length > 1;
        
        public int LastSelectedChapterIndex
        {
            get => PlayerPrefs.GetInt(LAST_SELECTED_CHAPTER_KEY, 0);
            private set => PlayerPrefs.SetInt(LAST_SELECTED_CHAPTER_KEY, value);
        }

        public int LastSelectedLevelIndex
        {
            get => PlayerPrefs.GetInt(LAST_SELECTED_LEVEL_KEY, 0);
            private set => PlayerPrefs.SetInt(LAST_SELECTED_LEVEL_KEY, value);
        }

        public int LastUnlockedChapterIndex
        {
            get => PlayerPrefs.GetInt(LAST_UNLOCKED_CHAPTER_KEY, _defaultUnlockedChapterIndex);
            private set => PlayerPrefs.SetInt(LAST_UNLOCKED_CHAPTER_KEY, value);
        }

        private string CurrentChapterKey => LastSelectedChapterIndex + "_";

        public int LastUnlockedLevelIndex
        {
            get => PlayerPrefs.GetInt(CurrentChapterKey + LAST_UNLOCKED_LEVEL_KEY, _defaultUnlockedLevelIndex);
            private set => PlayerPrefs.SetInt(CurrentChapterKey + LAST_UNLOCKED_LEVEL_KEY, value);
        }

        public int LevelText
        {
            get => PlayerPrefs.GetInt(LEVEL_TEXT_KEY, 1);
            private set => PlayerPrefs.SetInt(LEVEL_TEXT_KEY, value);
        }

        #endregion

        public LevelManager(GameDataHolderSO gameDataHolderSo, IViewBehaviourFactory viewBehaviourFactory)
        {
            var levelManagerData = gameDataHolderSo.levelManagerSo;
            _viewBehaviourFactory = viewBehaviourFactory;

            _chapterData = levelManagerData.chapterData;
            _defaultUnlockedChapterIndex = levelManagerData.defaultUnlockedChapterIndex;
            _defaultUnlockedLevelIndex = levelManagerData.defaultUnlockedLevelIndex;
        }

        ~LevelManager() => Dispose();

        public void Initialize()
        {
            _gameStateBinding.Add(OnStartGameStateTransition);
            EventBus<GameStateData>.AddListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));

            OnSelectChapter += SelectChapter;
            OnSelectLevel += SelectLevel;
        }

        public void Dispose()
        {
            _gameStateBinding.Remove(OnStartGameStateTransition);
            EventBus<GameStateData>.RemoveListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));

            OnSelectChapter -= SelectChapter;
            OnSelectLevel -= SelectLevel;
        }

        private bool SelectChapter(int selectedChapterIndex)
        {
            var isChapterExist = _chapterData.Length > selectedChapterIndex;

            if (isChapterExist)
            {
                LastSelectedChapterIndex = selectedChapterIndex;
            }
            else
            {
                Debug.LogError("Selected Chapter data does not exist!");
            }

            return isChapterExist;
        }

        private bool SelectLevel(int selectedLevelIndex)
        {
            var isLevelExist = _chapterData[LastSelectedChapterIndex].levelData.Length > selectedLevelIndex;

            if (isLevelExist)
            {
                LastSelectedLevelIndex = selectedLevelIndex;
            }
            else
            {
                Debug.LogError("Selected Level data does not exist!");
            }

            return isLevelExist;
        }

        private void OnStartGameStateTransition(GameStateData gameStateData)
        {
            if (gameStateData.EndState == GameState.GamePlayState)
            {
                if (_viewBehaviourFactory.TryGetModel<LevelUI, LevelModel>(out var levelModel))
                {
                    levelModel.UpdateLevelView(LevelText);
                }
            }
            
            if (gameStateData.EndState == GameState.GameSuccessState)
            {
                UpdateLevel();
            }
        }

        private void UpdateLevel()
        {
            if (IsThereMoreThanOneChapter)
            {
                if (LastSelectedChapterIndex != LastUnlockedChapterIndex || LastSelectedLevelIndex != LastUnlockedLevelIndex) return;
            }

            LevelText++;

            LastUnlockedLevelIndex++;

            if (LastUnlockedLevelIndex >= GetLastUnlockedChapterData().levelData.Length)
            {
                LastUnlockedChapterIndex++;

                if (LastUnlockedChapterIndex >= _chapterData.Length)
                {
                    LastUnlockedChapterIndex = _chapterData.Length - 1;

                    LastUnlockedLevelIndex = GetLastUnlockedChapterData().levelData.Length - 1;
                }
            }
        }

        public ChapterSO GetLastUnlockedChapterData() => _chapterData?[LastUnlockedChapterIndex];
        public LevelSO GetSelectedLevelData() => GetCurrentChapterData()?.levelData?[LastSelectedLevelIndex];

        public LevelSO GetCurrentLevelData() => GetCurrentChapterData()?.levelData?[LastUnlockedLevelIndex];
        public LevelSO GetPreviousLevelData() => GetCurrentChapterData()?.levelData?[LastUnlockedLevelIndex - 1];

        public ChapterSO GetCurrentChapterData() => _chapterData?[LastSelectedChapterIndex];
    }
}