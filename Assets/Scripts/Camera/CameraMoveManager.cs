using Player.ActionHandlers;
using UnityEngine;

namespace Camera
{
    public class CameraMoveManager : MonoBehaviour
    {

        private CameraMoveObserver _observer;
        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _observer = new CameraMoveObserver(DragAction, ClickHandler.Instance);
            _camera = CameraHolder.Instance.MainCamera;
        }

        private void OnEnable()
        {
            _observer.Subscribe();
        }

        private void OnDisable()
        {
            _observer.Unsubscribe();
        }


        private void DragAction(Vector3 obj)
        {
            Debug.Log("Move Camera");
        }


    }
}