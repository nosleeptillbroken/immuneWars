using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Takes images of gameObjects
/// </summary>
public class Imager : MonoBehaviour
{
    private Camera captureCamera = null;
    private Camera mainCamera = null;

    public GameObject gui = null;

    //
    void Start()
    {
        mainCamera = Camera.main;
        captureCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// Sets the Imaging Camera to be the active camera, and positions it for capture.
    /// </summary>
    /// <param name="target">The location to capture.</param>
    /// <param name="offset">Distance in 3D coordinates that the camera should be from the target.</param>
    void SetCamera(Vector3 target, Vector3 offset)
    {
        if(captureCamera != null)
        {
            captureCamera.transform.position = target + offset;
            captureCamera.transform.LookAt(target);

            mainCamera.enabled = false;
            captureCamera.enabled = true;
        }
    }

    /// <summary>
    /// Resets the camera back to the main camera.
    /// </summary>
    void ResetCamera()
    {
        if (captureCamera != null)
        {
            mainCamera.enabled = true;
            captureCamera.enabled = false;
        }
    }
    
    /// <summary>
    /// Captures the scene to a texture, and assigns a material's texture to the resulting texture.
    /// This method is asynchronous; the texture will load at the end of the current frame.
    /// </summary>
    /// <param name="target">The location to capture.</param>
    /// <param name="obj">The material to write the texture to.</param>
    /// <param name="textureWidth">Width of the generated texture.</param>
    /// <param name="textureHeight">Height of the generated texture.</param>
    /// <param name="offset">Distance in 3D coordinates that the camera should be from the target.</param>
    public void CaptureToObject(Vector3 target, GameObject obj, int textureWidth, int textureHeight, Vector3 offset)
    {
        Debug.Log("Capturing screenshot and saving to " + obj.name);

        object[] args = new object[] { target, obj, textureWidth, textureHeight, offset };
        StartCoroutine(RenderFrame(args));
    }

    IEnumerator RenderFrame(object[] args)
    {

        yield return new WaitForEndOfFrame();

        //

        Texture2D previousFrame = new Texture2D(mainCamera.pixelWidth, mainCamera.pixelHeight);
        previousFrame.ReadPixels(new Rect(0, 0, mainCamera.pixelWidth, mainCamera.pixelHeight), 0, 0);
        previousFrame.Apply();

        Vector3 target = (Vector3)args[0];
        GameObject obj = (GameObject)args[1];
        int textureWidth = (int)args[2];
        int textureHeight = (int)args[3];
        Vector3 offset = (Vector3)args[4];

        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);

        Vector3 screenPosition = captureCamera.WorldToScreenPoint(target);

        gui.SetActive(false);
        SetCamera(target, offset);

        yield return new WaitForEndOfFrame();

        //

        texture.ReadPixels(new Rect(screenPosition.x - textureWidth / 2, screenPosition.y - textureHeight / 2, Screen.width, Screen.height), 0, 0);
        
        GL.Clear(true, true, Color.black);
        Rect frameRect = new Rect(-Screen.width / 2, Screen.height / 2, Screen.width, -Screen.height);
        Debug.Log(frameRect);
        //GUI.DrawTexture(frameRect, previousFrame);

        gui.SetActive(true);
        ResetCamera();
        texture.Apply();

        obj.GetComponent<Renderer>().material.mainTexture = texture;
    }

}


