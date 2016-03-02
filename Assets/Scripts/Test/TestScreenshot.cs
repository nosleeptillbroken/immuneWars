using UnityEngine;
using System.Collections;

public class TestScreenshot : MonoBehaviour {

    public GameObject target;
    public Imager imager;
    
	void OnMouseDown()
    {
        imager.CaptureToObject(target.transform.position + new Vector3(0.0f,1.0f,0.0f), gameObject, Screen.height, Screen.height, new Vector3(0.0f, 3.0f, -3.75f));
	}
}
