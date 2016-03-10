using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager>
{
    #region Variables

    /// <summary>
    /// The current tower to spawn, or select, depending on the tower mode.
    /// </summary>
    public GameObject selectedTower = null;

    /// <summary>
    /// Minimum Y value towers can be placed at
    /// </summary>
    public float minYHeight = 0.0f;


    /// <summary>
    /// GameObject of the tower ghost. This is generated at runtime.
    /// </summary>
    public GameObject towerPlacementGhost = null;

    /// <summary>
    /// The UI panel that appears when a tower is selected.
    /// </summary>
    public GameObject towerSelectionPanel = null;
    
    /// <summary>
    /// The tower shop GameObject.
    /// </summary>
    public GameObject towerShop = null;

    #endregion

    #region Tower Modes

    [SerializeField] private bool _towerMode = false;
    public bool towerMode { get { return _towerMode; } }
    public bool placeTowers { get { return _towerMode; } }
    public bool selectTowers { get { return !_towerMode; } }

    //
    public void ToggleTowersMode()
    {
        if (_towerMode != true)
        {
            SetPlaceTowersMode();
        }
        else
        {
            SetSelectTowersMode();
        }
    }

    //
    public void SetPlaceTowersMode()
    {
        DeselectTower();
        if(_towerMode != true) towerShop.SendMessage("OpenPanel", null, SendMessageOptions.DontRequireReceiver);
        _towerMode = true;
    }

    //
    public void SetSelectTowersMode()
    {
        DeselectTower();
        if (_towerMode != false) towerShop.SendMessage("ClosePanel", null, SendMessageOptions.DontRequireReceiver);
        _towerMode = false;
    }

    #endregion

    #region Tower Selection

    /// <summary>
    /// Sets which tower the tower spawner will use to place towers. Also sets the ghost image for the tower spawner.
    /// </summary>
    /// <param name="selectedTower">The tower to be used. Must contain a tower component.</param>
    public void SelectTower(GameObject selectedTower)
    {
        if (selectedTower != null && selectedTower.GetComponent<TowerBehaviour>() != null)
        {
            this.selectedTower = selectedTower;
            if(placeTowers)
            {
                InitializeTowerGhost(this.selectedTower);
            }
            else
            {
                transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
                transform.rotation = selectedTower.transform.rotation;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
                if (towerSelectionPanel)
                {
                    towerSelectionPanel.SetActive(true);
                    towerSelectionPanel.SendMessage("OnSelectedTowerChange", null, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
        {
            DeselectTower();
        }
    }

    /// <summary>
    /// Attempts to select a tower based on the current mouse position
    /// </summary>
    void SelectTowerFromMousePosition()
    {
        // Raycast once
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // If raycast collides with a tower
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Towers"), QueryTriggerInteraction.Ignore))
        {
            GameObject rayTower = hit.collider.gameObject;

            if (rayTower)
            {
                SelectTower(rayTower);
                if (towerSelectionPanel) towerSelectionPanel.SendMessage("OnSelectedTowerChange", null, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    /// <summary>
    /// Deselects any tower selected by the tower manager
    /// </summary>
    public void DeselectTower()
    {
        selectedTower = null;
        if (towerPlacementGhost != null) Destroy(towerPlacementGhost.gameObject);
        if (towerSelectionPanel != null) towerSelectionPanel.SetActive(false);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    #endregion

    #region MonoBehaviour

    //
    void Start()
    {
        SelectTower(selectedTower);
    }

    //
    void Update()
    {
        if(_towerMode)
        {
            PlaceModeUpdate();
        }
        else
        {
            SelectModeUpdate();
        }
    }

    #endregion

    void PlaceModeUpdate()
    {
        if (Camera.main)
        {
            if (selectedTower != null && selectedTower.GetComponent<TowerBehaviour>())
            {
                // Raycast once per frame for tower location and for displaying tower ghost
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If raycast collides
                bool successfulRay = Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Towers", "Creeps"));

                if (towerPlacementGhost != null)
                {
                    if (successfulRay) // ignore towers and creeps in raycast so they don't obstruct terrain
                    {
                        // Set the ghost's position to the collision point of the ray
                        towerPlacementGhost.transform.position = hit.point;

                        // Orient the ghost so it's facing in the normal direction of the surface
                        Quaternion YY = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                        towerPlacementGhost.transform.rotation = Quaternion.LookRotation(hit.normal) * YY;

                        // Load the new tower's prefab
                        GameObject newTowerObj = selectedTower;

                        // Check if the tower would collide with any other towers
                        bool collidesWithtower = false;
                        if (newTowerObj != null)
                        {
                            // Get tower's collider component
                            CapsuleCollider capCollider = newTowerObj.GetComponent<CapsuleCollider>();

                            // Casts a sphere at the collision point that is the same radius as the tower's collider
                            // Returns true if colliding with tower
                            collidesWithtower = Physics.CheckSphere(hit.point, capCollider.radius * newTowerObj.transform.localScale.z, ~LayerMask.GetMask("Terrain")/*ignore terrain colliders*/, QueryTriggerInteraction.Ignore/*ignore range volumes*/);

                            // if tower does not collide
                            if (!collidesWithtower && hit.point.y > minYHeight)
                            {
                                // set ghost color to green to indicate tower can be placed
                                towerPlacementGhost.GetComponent<MeshRenderer>().material.color = new Color(0, 0.75f, 0, 0.5f);

                                // if mouse button is pressed and mouse is not over any GUI elements
                                if (Input.GetButtonDown("Place/Select Tower") && !EventSystem.current.IsPointerOverGameObject())
                                {
                                    // place the tower according to the ghost
                                    GameObject newTower = Instantiate(newTowerObj);
                                    newTower.transform.position = towerPlacementGhost.transform.position;
                                    newTower.transform.rotation = towerPlacementGhost.transform.rotation;

                                    // Deselect tower
                                    SelectTower(null);
                                }
                            }
                            else // otherwise
                            {
                                // set ghost color to red
                                towerPlacementGhost.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0, 0.5f);
                            }
                        }
                    }

                    // Turn off ghost if ray does not hit any objects
                    towerPlacementGhost.SetActive(successfulRay);
                }
            }
            else if (Input.GetButtonDown("Place/Select Tower") && !EventSystem.current.IsPointerOverGameObject())
            {
                SetSelectTowersMode();
                SelectTowerFromMousePosition();
            }
        }
    }

    void SelectModeUpdate()
    {
        if (Camera.main)
        {
            // Raycast once per frame
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If raycast collides with a tower
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Towers"), QueryTriggerInteraction.Ignore))
            {
                GameObject rayTower = hit.collider.gameObject;

                if (rayTower)
                {
                    // if mouse button is pressed and mouse is not over any GUI elements
                    if (Input.GetButtonDown("Place/Select Tower") && !EventSystem.current.IsPointerOverGameObject())
                    {
                        SelectTower(rayTower);
                    }
                }
            }
            else
            {
                // if mouse button is pressed and mouse is not over any GUI elements
                if (Input.GetButtonDown("Place/Select Tower") && !EventSystem.current.IsPointerOverGameObject())
                {
                    DeselectTower();
                }
            }

            if (towerSelectionPanel && selectedTower)
            {
                Vector3 panelPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position + (selectedTower.GetComponent<CapsuleCollider>().bounds.extents.y * selectedTower.transform.up));
                panelPosition.z = 0.0f;
                towerSelectionPanel.transform.position = panelPosition;
            }
        }
    }
    
    #region Tower Ghost

    public void InitializeTowerGhost(GameObject ghostTower)
    {
        if (ghostTower == null) return;

        // Destroy the old ghost
        if (towerPlacementGhost != null) Destroy(towerPlacementGhost.gameObject);

        // Load the ghost from the tower prefab
        towerPlacementGhost = Instantiate(ghostTower);

        // Destroy tower components so it's not functional
        Destroy(towerPlacementGhost.GetComponent<TowerBehaviour>());
        Destroy(towerPlacementGhost.GetComponent<Rigidbody>());
        Destroy(towerPlacementGhost.GetComponent<CapsuleCollider>());

        // Destroy children of the tower ghost
        foreach (Transform child in towerPlacementGhost.transform)
        {
            Destroy(child.gameObject);
        }

        // Load the ghost material
        towerPlacementGhost.GetComponent<MeshRenderer>().material = Resources.Load("Ghost") as Material;
        // Set it to the transparentfx layer
        towerPlacementGhost.layer = 1;
        // Remove the tag
        towerPlacementGhost.tag = "Untagged";

        // Set the name of the object to Tower Ghost
        towerPlacementGhost.name = "Tower Ghost";
    }

    #endregion

}
