using UnityEngine;
using System.Collections;

public class EventNotify : MonoBehaviour {

    void NotifyOf(string message)
    {
        Debug.Log(gameObject.name + ' ' + message + "()");
    }

	// Use this for initialization
	void Start ()
    {
        NotifyOf("Start");
	}
	
	// Update is called once per frame
	void Update ()
    {
        //NotifyOf("Update");
    }

    void OnEnable()
    {
        NotifyOf("OnEnable");
    }

    void OnDisable()
    {
        NotifyOf("OnDisable");
    }
}
