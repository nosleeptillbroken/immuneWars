using UnityEngine;
using System.Collections;

public class TowerSelector : MonoBehaviour {

    /// <summary>
    /// Tower the selector is focused on.
    /// </summary>
    public Tower selectedTower;

	// Use this for initialization
	void Start ()
    {
        // Focus on a tower if it is the current tower set
	    if(selectedTower)
        {
            Enable(selectedTower);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    /// <summary>
    /// Enables the selector.
    /// </summary>
    /// <param name="selectedTower">The tower the selector will be focused on</param>
    public void Enable(Tower selectedTower)
    {
        if (selectedTower)
        {
            this.selectedTower = selectedTower;
            transform.position = selectedTower.transform.position + new Vector3(0.0f, 5.0f, 0.0f);
            GetComponent<Projector>().enabled = true;
        }
    }

    /// <summary>
    /// Disables the selector.
    /// </summary>
    public void Disable()
    {
        this.selectedTower = null;
        transform.position = Vector3.zero;
        GetComponent<Projector>().enabled = false;
    }

}
