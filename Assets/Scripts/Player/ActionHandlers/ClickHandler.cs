using System;
using Camera;
using UnityEngine;
using Utils.Singleton;

namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;

        public event Action<Vector3> PointerDownEvent;
        public event Action<Vector3> PointerUpEvent;
        public event Action<Vector3> ClickEvent;

        public event Action<Vector3> DragStartEvent;
        public event Action<Vector3> DragEndEvent;
        /// <summary>
        /// Vector3 - it's a drag delta of screen position (in pixels)
        /// </summary>
        public event Action<Vector3> DragEvent;

        private Vector3 _pointerDownPosition;
        private Vector3 _pointerDragLastScreenPosition;
        private Vector3 _pointerDragScreenPosition;

        private bool _isClickProcess;
        private bool _isDragProcess;
        private bool _isDragged;

        private float _clickHoldDuration;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isClickProcess = true;
                _clickHoldDuration = .0f;

                _pointerDownPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

                PointerDownEvent?.Invoke(_pointerDownPosition);

                _pointerDownPosition = new Vector3(_pointerDownPosition.x, _pointerDownPosition.y, .0f);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var pointerUpPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

                if (_isDragProcess)
                {
                    DragEndEvent?.Invoke(pointerUpPosition);

                    _isDragProcess = false;
                }
                else
                {
                    ClickEvent?.Invoke(pointerUpPosition);
                }

                PointerUpEvent?.Invoke(pointerUpPosition);

                _isClickProcess = false;
            }
            else if (_isDragProcess)
            {
                _pointerDragScreenPosition = Input.mousePosition;
                _isDragged = _pointerDragScreenPosition != _pointerDragLastScreenPosition;

            }
        }

        private void LateUpdate()
        {
            if (_isClickProcess)
            {
                _clickHoldDuration += Time.deltaTime;
                if (_clickHoldDuration >= clickToDragDuration)
                {
                    DragStartEvent?.Invoke(_pointerDownPosition);

                    _isClickProcess = false;
                    _isDragProcess = true;

                    _pointerDragLastScreenPosition = _pointerDragScreenPosition = Input.mousePosition;
                }
            }
            else if (_isDragProcess)
            {
                if (_isDragged)
                {
                    DragEvent?.Invoke(_pointerDragScreenPosition - _pointerDragLastScreenPosition);
                    _pointerDragLastScreenPosition = _pointerDragScreenPosition;
                }
            }
        }

        public void AddDragEventHandlers(Action<Vector3> dragStartEvent, Action<Vector3> dragEndEvent)
        {
            DragStartEvent += dragStartEvent;
            DragEndEvent += dragEndEvent;
        }

        public void RemoveDragEventHandlers(Action<Vector3> dragStartEvent, Action<Vector3> dragEndEvent)
        {
            DragStartEvent -= dragStartEvent;
            DragEndEvent -= dragEndEvent;
        }

        public void ClearEvents()
        {
            DragStartEvent = null;
            DragEndEvent = null;
        }
    }
}