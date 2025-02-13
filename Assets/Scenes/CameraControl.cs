using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera mainCamera;  // Assign the camera in the inspector (optional)
    public bool isRendering = true;

    void Start()
    {
        // If the camera is not assigned, use the main camera by default
        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateRender();
    }

    void Update()
    {
        // Toggle visibility with the "V" key
        if (Input.GetKeyDown(KeyCode.V))
        {
            isRendering = !isRendering;

            UpdateRender();
        }
    }

    void UpdateRender()
    {
        if (isRendering)
        {
            // Render everything by setting the culling mask to "Everything"
            mainCamera.cullingMask = -1;  // -1 means all layers are rendered
        }
        else
        {
            // Render nothing by setting the culling mask to 0
            mainCamera.cullingMask = 0;   // 0 means no layers are rendered
        }
    }
}
