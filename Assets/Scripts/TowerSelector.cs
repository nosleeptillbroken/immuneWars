using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSelector : MonoBehaviour {

    public EventSystem eventSystem;

    /// <summary>
    /// Tower the selector is focused on.
    /// </summary>
    public TowerBehaviour selectedTower = null;

    /// <summary>
    /// The UI panel that appears when a tower is selected
    /// </summary>
    public GameObject selectionPanel = null;

	// Use this for initialization
	void Start ()
    {
        // Focus on a tower if it is the current tower set
        SelectTower(selectedTower);
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
            TowerBehaviour rayTower = hit.collider.gameObject.GetComponent<TowerBehaviour>();

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

        if(selectionPanel && selectedTower)
        {
            Vector3 panelPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position + (selectedTower.GetComponent<CapsuleCollider>().bounds.extents.y * selectedTower.transform.up));
            panelPosition.z = 0.0f;
            selectionPanel.transform.position = panelPosition;
        }
    }


    /// <summary>
    /// Enables the selector.
    /// </summary>
    /// <param name="selectedTower">The tower the selector will be focused on</param>
    public void SelectTower(TowerBehaviour selectedTower)
    {
        if (selectedTower)
        {
            this.selectedTower = selectedTower;
            transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
            transform.rotation = selectedTower.transform.rotation;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            if (selectionPanel)
            {
                selectionPanel.SetActive(true);
                selectionPanel.GetComponent<TowerSelectionPanel>().UpdateDisplayInformation();
            }
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
        if (selectionPanel)
        {
            selectionPanel.SetActive(false);
        }
        this.selectedTower = null;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
