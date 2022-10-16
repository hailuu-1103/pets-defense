namespace Common
{
    using UnityEngine;

    public class CameraSizeHelper : MonoBehaviour
    {
        [SerializeField] private float horizontalScreenSizeOffset; // Max distance of screen size in horizontal
        
        private Camera mainCamera;
        private void Start()
        {
            this.mainCamera = Camera.main;
            this.SetCamSize();
        }
        private void SetCamSize()
        {
            var unitsPerPixel     = this.horizontalScreenSizeOffset / Screen.width;
            var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            this.mainCamera.orthographicSize = desiredHalfHeight;
        }
    }
}