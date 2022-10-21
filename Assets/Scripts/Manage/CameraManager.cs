namespace Manage
{
    using UnityEngine;

    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private float   cameraSpeed = 20f;
        private                  Camera  mainCamera;
        private                  Vector2 firstTouchPos;
        private                  Vector2 currentTouchPos;
        private                  Vector2 touchPos;

        private void Start()
        {
            this.mainCamera = Camera.main;
        }
        private void Update()
        {
            
        }
    }
}