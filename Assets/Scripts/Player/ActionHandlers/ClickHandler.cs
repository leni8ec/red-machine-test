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
        /// <summary>
        /// Use a screen position (in pixels) 
        /// </summary>
        private Vector3 _pointerDragLastScreenPosition;

        private bool _isClickProcess;
        private bool _isDragProcess;

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
            else if (_isClickProcess)
            {
                _clickHoldDuration += Time.deltaTime;
                if (_clickHoldDuration >= clickToDragDuration)
                {
                    _isClickProcess = false;
                    _isDragProcess = true;

                    DragStartEvent?.Invoke(_pointerDownPosition);

                    // reset last drag screen position for current event
                    _pointerDragLastScreenPosition = Input.mousePosition;
                }
            }
            else if (_isDragProcess)
            {
                var pointerDragScreenPosition = Input.mousePosition;
                bool isDragged = pointerDragScreenPosition != _pointerDragLastScreenPosition;
                if (isDragged)
                {
                    DragEvent?.Invoke(pointerDragScreenPosition - _pointerDragLastScreenPosition);

                    _pointerDragLastScreenPosition = pointerDragScreenPosition;
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