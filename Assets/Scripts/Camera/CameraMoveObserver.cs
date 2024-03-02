using System;
using Events;
using Player.ActionHandlers;
using UnityEngine;

namespace Camera
{
    [DefaultExecutionOrder(1)] // Must be executed after 'ColorConnectionManager'
    public class CameraMoveObserver
    {

        private readonly Action<Vector3> _dragAction;
        private readonly Action _dragEndAction;
        private readonly Action _resetAction;
        private readonly ClickHandler _clickHandler;

        private bool _freeToMove;

        public CameraMoveObserver(Action<Vector3> dragAction, Action dragEndAction, Action resetAction, ClickHandler clickHandler)
        {
            _dragAction = dragAction;
            _dragEndAction = dragEndAction;
            _resetAction = resetAction;
            _clickHandler = clickHandler;
        }


        public void Subscribe()
        {
            _clickHandler.DragEvent += DragEventHandler;
            _clickHandler.DragEndEvent += DragEndEventHandler;

            EventsController.Subscribe<EventModels.Game.BackgroundTapped>(this, OnBackgroundTapped);
            EventsController.Subscribe<EventModels.Game.PlayerFingerRemoved>(this, OnPlayerFingerRemoved);
            EventsController.Subscribe<EventModels.Game.TargetColorNodesFilled>(this, OnLevelCompleted);
        }


        public void Unsubscribe()
        {
            _clickHandler.DragEvent -= DragEventHandler;
            _clickHandler.DragEndEvent -= DragEndEventHandler;

            EventsController.Unsubscribe<EventModels.Game.BackgroundTapped>(OnBackgroundTapped);
            EventsController.Unsubscribe<EventModels.Game.PlayerFingerRemoved>(OnPlayerFingerRemoved);
            EventsController.Unsubscribe<EventModels.Game.TargetColorNodesFilled>(OnLevelCompleted);
        }

        private void DragEventHandler(Vector3 position)
        {
            if (!_freeToMove) return;

            _dragAction?.Invoke(position);
        }
        
        private void DragEndEventHandler(Vector3 position)
        {
            _dragEndAction?.Invoke();
        }


        private void OnPlayerFingerRemoved(EventModels.Game.PlayerFingerRemoved e)
        {
            _freeToMove = false;
        }

        private void OnBackgroundTapped(EventModels.Game.BackgroundTapped e)
        {
            _freeToMove = true;
        }

        private void OnLevelCompleted(EventModels.Game.TargetColorNodesFilled e)
        {
            _resetAction?.Invoke();
        }

    }
}