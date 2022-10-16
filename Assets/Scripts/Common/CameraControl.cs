using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera control script.
/// </summary>
public class CameraControl : MonoBehaviour
{
    // Start view width
    private int defaultWidth;
    // Width on last change
    private int lastUpdatedWidth;
    // Start camera size
    private float defaultCameraSize;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        defaultWidth = lastUpdatedWidth = Camera.main.pixelWidth;
        defaultCameraSize = Camera.main.orthographicSize;
        ChangeCameraSize();
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (lastUpdatedWidth != Camera.main.pixelWidth)
        {
            lastUpdatedWidth = Camera.main.pixelWidth;
            ChangeCameraSize();
        }
    }

    /// <summary>
    /// Change camera size to keep same view width.
    /// </summary>
    private void ChangeCameraSize()
    {
        Camera.main.orthographicSize = defaultCameraSize * ((float)defaultWidth / (float)lastUpdatedWidth);
    }
}
