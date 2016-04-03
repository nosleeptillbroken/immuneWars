using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TowerManager : MonoSingleton<TowerManager>
{
    #region Variables

    /// <summary>
    /// The current tower to spawn, or select, depending on the tower mode.
    /// </summary>
    public GameObject selectedTower = null;


    /// <summary>
    /// The circle that indicates the tower's range.
    /// </summary>
    public GameObject towerRangeCircle = null;

    /// <summary>
    /// GameObject of the tower ghost. This is generated at runtime.
    /// </summary>
    public GameObject towerPlacementGhost = null;

    /// <summary>
    /// The GameObject that highlights the tower when it is selected.
    /// </summary>
    public GameObject towerSelectionHighlight = null;

    /// <summary>
    /// The UI panel that appears when a tower is selected.
    /// </summary>
    public GameObject towerSelectionPanel = null;
    
    /// <summary>
    /// The tower shop GameObject.
    /// </summary>
    public GameObject towerShop = null;

    [Header("Placement Restrictions")]

    /// <summary>
    /// Minimum Y value towers can be placed at
    /// </summary>
    public float minYHeight = 0.0f;

    /// <summary>
    /// The difference in angle that the surface normal can be from global up.
    /// </summary>
    [Range(0.0f,360.0f)] public float angleTolerance = 5.0f;

    /// <summary>
    /// The strength (or greater) the tower placement splat texture (texture 0) must be for the tower to be placeable.
    /// </summary>
    [Range(0.0f,1.0f)] public float splatTextureTolerance = 0.75f;

    #endregion

    #region Tower Modes

    private bool _towerMode = false;
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
        if (_towerMode != true && towerShop != null) towerShop.SendMessage("OpenPanel", null, SendMessageOptions.DontRequireReceiver);
        DeselectTower();
        _towerMode = true;
    }

    //
    public void SetSelectTowersMode()
    {
        if (_towerMode != false && towerShop != null) towerShop.SendMessage("ClosePanel", null, SendMessageOptions.DontRequireReceiver);
        DeselectTower();
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
        if (selectedTower != null && (selectedTower.GetComponent<TowerBehaviour>() != null || selectedTower.GetComponent<LevelData>() != null))
        {
            this.selectedTower = selectedTower;
            if(placeTowers)
            {
                InitializeTowerGhost();
            }
            else
            {

                towerRangeCircle.transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
                towerSelectionHighlight.SetActive(true);

                towerSelectionHighlight.transform.position = selectedTower.transform.position + (selectedTower.transform.up * 0.01f);
                towerSelectionHighlight.transform.rotation = selectedTower.transform.rotation;
                towerSelectionHighlight.SetActive(true);

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

        if (towerPlacementGhost != null)
        {
            towerPlacementGhost.SetActive(false);
        }

        if (towerSelectionHighlight != null)
        {
            towerSelectionHighlight.SetActive(false);
        }

        if (towerSelectionPanel != null)
        {
            towerSelectionPanel.SetActive(false);
        }
        
        if (towerRangeCircle != null)
        {
            towerRangeCircle.SetActive(false);
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
                bool successfulRay = Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Towers", "Creeps", "Paths"));

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
                        bool canPlacePoint = false;
                        if (newTowerObj != null)
                        {
                            // Get tower's collider component
                            CapsuleCollider capCollider = newTowerObj.GetComponent<CapsuleCollider>();

                            // Casts a sphere at the collision point that is the same radius as the tower's collider
                            // Returns true if colliding with tower
                            collidesWithtower = Physics.CheckSphere(hit.point, capCollider.radius * newTowerObj.transform.localScale.z, ~LayerMask.GetMask("Terrain")/*ignore terrain colliders*/, QueryTriggerInteraction.Ignore/*ignore range volumes*/);
                            
                            // check that the angle of the tower is within 45 degrees of the global up
                            canPlacePoint = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) < (angleTolerance * Mathf.Deg2Rad);

                            // check if the point is above the minimum y value
                            canPlacePoint = canPlacePoint && (hit.point.y > minYHeight);

                            // if the ray hit a terrain component
                            Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                            if(terrain)
                            {
                                // if the splat texture is mostly the tower placement texture
                                canPlacePoint = canPlacePoint && (GetTerrainValue(terrain, hit.point, 0) >= splatTextureTolerance);
                            }
                            
                            // if tower does not collide
                            if (!collidesWithtower && canPlacePoint)
                            {
                                // set ghost color to green to indicate tower can be placed
                                Color canPlace = new Color(0, 0.75f, 0, 0.5f);

                                towerPlacementGhost.GetComponent<MeshRenderer>().material.color = canPlace;
                                towerRangeCircle.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", canPlace);

                                // if mouse button is pressed and mouse is not over any GUI elements
                                if (Input.GetButtonDown("Place/Select Tower") && !EventSystem.current.IsPointerOverGameObject())
                                {
                                    // place the tower according to the ghost
                                    GameObject newTower = Instantiate(newTowerObj);
                                    newTower.transform.position = towerPlacementGhost.transform.position;
                                    newTower.transform.rotation = towerPlacementGhost.transform.rotation;

                                    // Deselect tower
                                    DeselectTower();
                                }
                            }
                            else // otherwise
                            {
                                // set ghost color to red
                                Color cannotPlace = new Color(0.75f, 0, 0, 0.5f);

                                towerPlacementGhost.GetComponent<MeshRenderer>().material.color = cannotPlace;
                                towerRangeCircle.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", cannotPlace);
                            }
                        }
                    }

                    // turn off ghost if no tower is selected
                    successfulRay = successfulRay && selectedTower;

                    // Turn off ghost if ray does not hit any objects
                    towerPlacementGhost.SetActive(successfulRay);

                    // set the tower range circle to active if there is a selected tower
                    towerRangeCircle.SetActive(selectedTower != null);

                    if (selectedTower)
                    {
                        // update position and scale of the tower range circle
                        TowerBehaviour selectedTowerBehaviour = selectedTower.GetComponent<TowerBehaviour>();
                        towerRangeCircle.transform.localScale = Vector3.one * (selectedTowerBehaviour.compositeAttributes.range) * 2.0f;
                        towerRangeCircle.transform.position = hit.point + new Vector3(0.0f, 0.01f, 0.0f);
                    }
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

            // set the tower range circle to active if there is a selected tower
            towerRangeCircle.SetActive(selectedTower != null);

            if (selectedTower)
            {
                towerRangeCircle.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green);
                TowerBehaviour selectedTowerBehaviour = selectedTower.GetComponent<TowerBehaviour>();
                if (selectedTowerBehaviour)
                {
                    towerRangeCircle.transform.localScale = Vector3.one * selectedTowerBehaviour.compositeAttributes.range * 2.0f;
                    towerRangeCircle.transform.position = selectedTower.transform.FindChild("RangeVolume").transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                }
            }
        }
    }

    #region Terrain Placement

    float GetTerrainValue(Terrain terrain, Vector3 worldPos, int splatTextureId)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        return (splatTextureId < splatmapData.GetLength(2)) ? splatmapData[0, 0, splatTextureId] : 0.0f;
    }

    #endregion

    #region TowerGhost

    void InitializeTowerGhost()
    {
        TowerBehaviour selectedTowerBehaviour = selectedTower.GetComponent<TowerBehaviour>();

        towerPlacementGhost.GetComponent<MeshFilter>().mesh = selectedTower.GetComponent<MeshFilter>().sharedMesh;
        
        towerRangeCircle.transform.localScale = Vector3.one * selectedTowerBehaviour.attributes.range * 2.0f;
        towerRangeCircle.transform.localPosition = selectedTower.transform.FindChild("RangeVolume").transform.localPosition + new Vector3(0.0f, 0.01f, 0.0f);
    }

    #endregion

}
