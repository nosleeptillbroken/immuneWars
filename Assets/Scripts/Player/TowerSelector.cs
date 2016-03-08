using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSelector : Singleton<TowerSelector>
{ 
    /// <summary>
    /// Tower the selector is focused on.
    /// </summary>
    private TowerBehaviour _selectedTower = null;
    public TowerBehaviour selectedTower { get { return _selectedTower; } }

    /// <summary>
    /// The UI panel that appears when a tower is selected
    /// </summary>
    [SerializeField] private GameObject _selectionPanel = null;
    public GameObject selectionPanel { get { return _selectionPanel; } }
    

	// Use this for initialization
	void Start ()
    {
        // Focus on a tower if it is the current tower set
        SelectTower(_selectedTower);
	}

    //
    void OnDisable()
    {
        DeselectTower();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Camera.main)
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
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        SelectTower(rayTower);
                    }
                }
            }
            else
            {
                // if mouse button is pressed and mouse is not over any GUI elements
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    DeselectTower();
                }
            }

            if (_selectionPanel && _selectedTower)
            {
                Vector3 panelPosition = Camera.main.WorldToScreenPoint(_selectedTower.transform.position + (_selectedTower.GetComponent<CapsuleCollider>().bounds.extents.y * _selectedTower.transform.up));
                panelPosition.z = 0.0f;
                _selectionPanel.transform.position = panelPosition;
            }
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
            this._selectedTower = selectedTower;
            transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
            transform.rotation = selectedTower.transform.rotation;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            if (_selectionPanel && selectedTower)
            {
                _selectionPanel.SetActive(true);
                _selectionPanel.GetComponent<TowerSelectionPanel>().UpdateDisplayInformation();
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
        if (_selectionPanel)
        {
            _selectionPanel.SetActive(false);
        }
        this._selectedTower = null;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
