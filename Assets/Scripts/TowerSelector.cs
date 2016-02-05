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
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        // Focus on a tower if it is the current tower set
	    if(selectedTower)
        {
            SelectTower(selectedTower);
        }
	}

    //
    void OnDisable()
    {
        DeselectTower();
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
        else
        {
            // if mouse button is pressed and mouse is not over any GUI elements
            if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
            {
                DeselectTower();
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
            transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
            transform.rotation = selectedTower.transform.rotation;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            DeselectTower();
        }
    }

    /// <summary>
    /// Disables the selector.
    /// </summary>
    public void DeselectTower()
    {
        this.selectedTower = null;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

}
