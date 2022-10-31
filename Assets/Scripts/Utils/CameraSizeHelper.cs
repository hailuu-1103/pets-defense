namespace Utils
{
    using UnityEngine;

    public class CameraSizeHelper : MonoBehaviour
    {
        [SerializeField] private float horizontalScreenOffset; // Max distance of screen size in horizontal
        
        private Camera mainCamera;
        private void Start()
        {
            this.mainCamera = Camera.main;
            this.SetCamSize();
        }
        private void SetCamSize()
        {
            var unitsPerPixel     = this.horizontalScreenOffset / Screen.width;
            var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            this.mainCamera.orthographicSize = desiredHalfHeight;
        }
    }
}