using Events;
using UnityEngine;
using Utils.Scenes;
using Utils.Singleton;

namespace Levels
{
    public class LevelsManager : DontDestroyMonoBehaviour
    {
        private const string LevelNamePattern = "Level{0}";

        [SerializeField]
        private int startLevelIndex;

        private int _currentLevelIndex;


        private void Start()
        {
            _currentLevelIndex = startLevelIndex;
            ScenesChanger.GotoScene(string.Format(LevelNamePattern, _currentLevelIndex));

            EventsController.Subscribe<EventModels.Game.TargetColorNodesFilled>(this, OnTargetColorNodesFilled);
        }

        private void OnTargetColorNodesFilled(EventModels.Game.TargetColorNodesFilled e)
        {
            _currentLevelIndex += 1;
            ScenesChanger.GotoScene(string.Format(LevelNamePattern, _currentLevelIndex));
        }
    }
}