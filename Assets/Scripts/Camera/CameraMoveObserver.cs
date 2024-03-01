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
        private readonly ClickHandler _clickHandler;

        private bool _freeToMove;

        public CameraMoveObserver(Action<Vector3> dragAction, ClickHandler clickHandler)
        {
            _dragAction = dragAction;
            _clickHandler = clickHandler;
        }


        public void Subscribe()
        {
            _clickHandler.DragEvent += DragEventHandler;

            EventsController.Subscribe<EventModels.Game.BackgroundTapped>(this, OnBackgroundTappedEvent);
            EventsController.Subscribe<EventModels.Game.PlayerFingerRemoved>(this, OnPlayerFingerRemovedEvent);
        }

        public void Unsubscribe()
        {
            _clickHandler.DragEvent -= DragEventHandler;

            EventsController.Unsubscribe<EventModels.Game.BackgroundTapped>(OnBackgroundTappedEvent);
            EventsController.Unsubscribe<EventModels.Game.PlayerFingerRemoved>(OnPlayerFingerRemovedEvent);
        }

        private void OnPlayerFingerRemovedEvent(EventModels.Game.PlayerFingerRemoved tapped)
        {
            _freeToMove = false;
        }

        private void OnBackgroundTappedEvent(EventModels.Game.BackgroundTapped tapped)
        {
            _freeToMove = true;
        }

        private void DragEventHandler(Vector3 position)
        {
            if (!_freeToMove) return;

            _dragAction?.Invoke(position);
        }

    }
}