using Player.ActionHandlers;
using UnityEngine;

namespace Camera
{
    public class CameraMoveManager : MonoBehaviour
    {

        [SerializeField] private float cameraMoveMultiplier = 0.01f;
        [SerializeField] private float cameraLerpMultiplier = 5f;
        [Tooltip("In screen pixels")]
        [SerializeField] private Vector2 maxMoveOffset = new Vector2(3, 5);

        private CameraMoveObserver _observer;
        private Transform _cameraTransform;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;

        // used for fast transitions after drag end
        private const float CameraFastLerpMultiplier = 10;
        private bool _isFastMove;


        private void Awake()
        {
            _observer = new CameraMoveObserver(DragAction, DragEndAction, ResetAction, ClickHandler.Instance);
            _cameraTransform = CameraHolder.Instance.MainCamera.transform;
            _startPosition = _cameraTransform.position;
            _targetPosition = _startPosition;
        }

        private void OnEnable()
        {
            _observer.Subscribe();
        }

        private void OnDisable()
        {
            _observer.Unsubscribe();
        }


        private void DragAction(Vector3 dragDelta)
        {
            // move camera
            _targetPosition -= cameraMoveMultiplier * dragDelta;
            ClampTargetPosition();

            if (_isFastMove) _isFastMove = false;
        }

        private void ClampTargetPosition()
        {
            // clamp to camera offset limits
            var cameraOffset = _targetPosition - _startPosition;
            if (Mathf.Abs(cameraOffset.x) > maxMoveOffset.x) _targetPosition.x = cameraOffset.x < 0 ? -maxMoveOffset.x : maxMoveOffset.x;
            if (Mathf.Abs(cameraOffset.y) > maxMoveOffset.y) _targetPosition.y = cameraOffset.y < 0 ? -maxMoveOffset.y : maxMoveOffset.y;
            // reset z coordinate
            _targetPosition.z = _startPosition.z;
        }

        private void DragEndAction()
        {
            _isFastMove = true;
        }

        private void ResetAction()
        {
            _cameraTransform.position = _startPosition;

            // Script must be placed on main camera fro this case
            // otherwise it's destroyed before the camera has a chance to move.
            // _targetPosition = _startPosition;
        }


        private void Update()
        {
            bool meaningfulOffset = Vector3.Distance(_cameraTransform.position, _targetPosition) > 0.1f;
            if (!meaningfulOffset) return;

            float t = Time.deltaTime * (_isFastMove ? CameraFastLerpMultiplier : cameraLerpMultiplier);
            var lerpPosition = Vector3.Lerp(_cameraTransform.position, _targetPosition, t);
            _cameraTransform.position = lerpPosition;
        }

    }
}