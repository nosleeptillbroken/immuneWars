// CameraController.cs
// Script allows for RTS-style camera movement, rotation, and zoom.

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    // Pan Properties
    [Header("Panning")]
    [Tooltip("Enables whether hovering the mouse near a screen edge pans the camera in that direction.")]
    public bool enableEdgePanning = true;
    [Tooltip("Distance from the edge of the screen before edge panning will occur.")]
    public float edgePanThreshold = 16; // The number of px on each side of the screen that will count towards panning the screen
    public float panSpeed = 32; // The speed at which the camera pans

    [Tooltip("Enables camera restriction within a box defined by two points.")]
    public bool cameraBoundsRestriction = false;
    public Vector3 minimumBounds = Vector3.zero;
    public Vector3 maximumBounds = Vector3.zero;

    // Rotation Properties
    [Header("Rotation")]

    public bool enableRotation = true;
    public float rotationSensitivity = 90;
    public float rotationSpeed = 45; // The speed at which the camera rotates during zoom
                                    //public float rotationThreshold = 8; // The threshold at which rotation stops. larger number = more narrow range of rotation

    // Zoom Properties
    [Header("Zoom")]

    public bool enableZoom = true;
    public float zoomSensitivity = 18;
    public float zoomSpeed = 4;

    private float zoom = 0.5f;
    private float zoomLerp = 0;

    public Vector3 zoomedInOrientation = Vector3.zero;
    public Vector3 zoomedOutOrientation = new Vector3(90, 0, 0);
    private Quaternion zoomedInRot, zoomedOutRot;

    public float zoomedInHeight = 2;
    public float zoomedOutHeight = 20;
    

    Quaternion currRot = Quaternion.identity;
    
    bool freeRotate = false;
    float xDeg = 0;
    float yDeg = 0;

    // Use this for initialization
    void Start()
    {
        zoomLerp = zoom;
        zoomedInRot = Quaternion.Euler(zoomedInOrientation);
        zoomedOutRot = Quaternion.Euler(zoomedOutOrientation);
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse position on screen
        float cursorX = Input.mousePosition.x;
        float cursorY = Input.mousePosition.y;
        
        // Get change in position for mouse
        float deltaMouseX = Input.GetAxis("Mouse X");
        float deltaMouseY = Input.GetAxis("Mouse Y");

        // Get current orientation and translation
        currRot = transform.rotation;

        // Lock mouse to remain within screen
        Cursor.lockState = CursorLockMode.Confined;

        // Free movement w/ Middle Mouse Click
        // If middle mouse button is pressed
        if(Input.GetMouseButtonDown(1))
        {
            xDeg = transform.rotation.eulerAngles.x;
            yDeg = transform.rotation.eulerAngles.y;
            freeRotate = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            freeRotate = false;
        }

		// If freely rotating follow the change in mousePosition with camera rotation
        if (freeRotate)
        {
            xDeg -= deltaMouseY * Time.deltaTime * rotationSensitivity;
            yDeg += deltaMouseX * Time.deltaTime * rotationSensitivity;
            transform.rotation = Quaternion.Euler(xDeg, yDeg, 0);
        }
        else if (Input.GetMouseButton(2))
        {
            // Set rotation to identity so that translation is planar
            transform.rotation = Quaternion.identity;
            // Translate camera according to mouse delta
            transform.Translate(Vector3.right * -deltaMouseX * Time.deltaTime * panSpeed);
            transform.Translate(Vector3.forward * -deltaMouseY * Time.deltaTime * panSpeed);
            // Reset Rotation to initial
            transform.rotation = currRot;
        }
        else // Control camera with either WSAD keys or moving mouse to the edge
        {
            float sprintMult = 1.0f;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                sprintMult = 0.34f;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                sprintMult = 2.0f;
            }
            if ((cursorX < edgePanThreshold && enableEdgePanning) || Input.GetKey(KeyCode.A))
            {
                transform.rotation = Quaternion.identity;
                transform.Translate(Vector3.right * -panSpeed * Time.deltaTime * sprintMult);
                transform.rotation = currRot;
            }
            else if ((cursorX >= Screen.width - edgePanThreshold && enableEdgePanning) || Input.GetKey(KeyCode.D))
            {
                transform.rotation = Quaternion.identity;
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime * sprintMult);
                transform.rotation = currRot;
            }

            if ((cursorY < edgePanThreshold && enableEdgePanning) || Input.GetKey(KeyCode.S))
            {
                transform.rotation = Quaternion.identity;
                transform.Translate(Vector3.forward * -panSpeed * Time.deltaTime * sprintMult);
                transform.rotation = currRot;
            }
            else if ((cursorY >= Screen.height - edgePanThreshold && enableEdgePanning) || Input.GetKey(KeyCode.W))
            {
                transform.rotation = Quaternion.identity;
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime * sprintMult);
                transform.rotation = currRot;
            }
        }

		// Get mouse scroll wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
		// If scroll wheel moved zoom camera in relation
		if(scroll != 0)
        {
            zoom += zoomSensitivity * Time.deltaTime * -scroll;
        }
        zoom = Mathf.Clamp(zoom, 0.0f, 1.0f);

    }

	// LateUpdate is called at the end of every frame, after update
    void LateUpdate()
    {
        float zoomPercent = Mathf.Clamp(Time.deltaTime * zoomSpeed, 0.0f, 1.0f);
		
		// Adjust the zoom, rotation, y and z coordinates of the camera according to changes this frame
		zoomLerp = Mathf.Lerp(zoomLerp, zoom, zoomPercent);
        if (!freeRotate)
        {
            transform.rotation = Quaternion.Slerp(zoomedInRot, zoomedOutRot, zoomLerp);
        }
        Vector3 hpos = transform.position;
        hpos.y = Mathf.Lerp(zoomedInHeight, zoomedOutHeight, zoomLerp);
        hpos.z += (zoom - zoomLerp) * zoomSensitivity * Time.deltaTime;

        transform.position = hpos;
		
		// Bound the camera position so it cannot fly away from the play area
        if(cameraBoundsRestriction)
        {
            Vector3 clampPos;
            clampPos.x = Mathf.Clamp(hpos.x, minimumBounds.x, maximumBounds.x);
            clampPos.y = Mathf.Clamp(hpos.y, minimumBounds.y, maximumBounds.y);
            clampPos.z = Mathf.Clamp(hpos.z, minimumBounds.z, maximumBounds.z);

            transform.position = clampPos;
        }

    }
}
