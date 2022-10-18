namespace Common
{
    using UnityEngine;

    public class CameraSizeHelper : MonoBehaviour
    {
        [SerializeField] private float verticleScreenSizeOffset; // Max distance of screen size in horizontal
        
        private Camera mainCamera;
        private void Start()
        {
            this.mainCamera = Camera.main;
            this.SetCamSize();
        }
        private void SetCamSize()
        {
            var unitsPerPixel     = this.verticleScreenSizeOffset / Screen.height;
            var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.width;
            this.mainCamera.orthographicSize = desiredHalfHeight;
        }
    }
}