using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour {

    public EventSystem eventSystem;

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
            SelectTower(selectedTower);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Raycast once per frame
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // If raycast collides with a tower
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Towers"), QueryTriggerInteraction.Ignore))
        {
            Tower rayTower = hit.collider.gameObject.GetComponent<Tower>();

            if (rayTower)
            {
                // if mouse button is pressed and mouse is not over any GUI elements
                if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
                {
                    SelectTower(rayTower);
                }
            }
        }
    }


    /// <summary>
    /// Enables the selector.
    /// </summary>
    /// <param name="selectedTower">The tower the selector will be focused on</param>
    public void SelectTower(Tower selectedTower)
    {
        if (selectedTower)
        {
            this.selectedTower = selectedTower;
            transform.position = selectedTower.transform.position + new Vector3(0.0f, 5.01f, 0.0f);
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponentInChildren<Projector>().enabled = true;
        }
    }

    /// <summary>
    /// Disables the selector.
    /// </summary>
    public void DeselectTower()
    {
        this.selectedTower = null;
        transform.position = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponentInChildren<Projector>().enabled = false;
    }

}
