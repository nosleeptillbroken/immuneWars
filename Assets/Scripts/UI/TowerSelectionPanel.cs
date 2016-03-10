using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerSelectionPanel : MonoBehaviour
{

    public GameObject selectedTower { get { return TowerManager.instance.selectedTower; } }
    public TowerBehaviour selectedTowerBehaviour { get { return selectedTower ? selectedTower.GetComponent<TowerBehaviour>() : null; } }

    private Text displayNameUI;
    private Dropdown targetingModeUI;
    private Toggle sortOrderUI;

    public Button[] upgradeButtonsUI;

    /// <summary>
    /// Destroys the selected tower without selling it.
    /// </summary>
    public void DestroySelectedTower()
    {
        if(TowerManager.instance && selectedTower)
        {
            Destroy(selectedTower.gameObject);
            TowerManager.instance.DeselectTower();
        }
    }

    /// <summary>
    /// Sells the selected tower.
    /// </summary>
    public void SellSelectedTower()
    {
        if(TowerManager.instance)
        {
            Player.instance.AddGold(selectedTowerBehaviour.attributes.cost + selectedTowerBehaviour.upgradeAttributes.cost);
            DestroySelectedTower();
        }
    }

    public void UpgradeSelectedTower(int path)
    {
        if(TowerManager.instance && selectedTower && Player.instance.RemoveGold(selectedTowerBehaviour.attributes.cost))
        {
            selectedTowerBehaviour.Upgrade(path);
            UpdateDisplayInformation();
        }
    }

    /// <summary>
    /// Updates the information displayed on the selection panel with the information from the tower and its attributes.
    /// </summary>
    public void UpdateDisplayInformation()
    {
        if (!HasComponents())
        {
            GetComponents();
        }

        if (selectedTower)
        {
            TowerAttributes compositeAttributes = selectedTowerBehaviour.attributes + selectedTowerBehaviour.upgradeAttributes;

            displayNameUI.text = compositeAttributes.displayName;

            targetingModeUI.value = (int)selectedTowerBehaviour.targetingMode;

            sortOrderUI.isOn = selectedTowerBehaviour.sortDescending;
            sortOrderUI.gameObject.GetComponentInChildren<Text>().text = sortOrderUI.isOn ? ">" : "<";

            int i = 0;
            foreach (Button btn in upgradeButtonsUI)
            {
                if (selectedTowerBehaviour.CanUpgrade(i))
                {
                    btn.transform.FindChild("Name").GetComponent<Text>().text = selectedTowerBehaviour.GetNextUpgrade(i).displayName;
                    btn.transform.FindChild("Cost").gameObject.SetActive(true);
                    btn.transform.FindChild("Cost").GetComponent<Text>().text = "Cost: " + selectedTowerBehaviour.GetNextUpgrade(i).cost;
                    if (Player.instance.currentGold >= selectedTowerBehaviour.GetNextUpgrade(i).cost)
                    {
                        btn.interactable = true;
                    }
                    else
                    {
                        btn.interactable = false;
                    }
                }
                else
                {
                    btn.transform.FindChild("Name").GetComponent<Text>().text = "Complete";
                    btn.transform.FindChild("Cost").gameObject.SetActive(false);
                    btn.interactable = false;
                }
                i += 1;
            }
        }
    }


    /// <summary>
    /// Updates the targeting mode according to the targeting dropdown.
    /// </summary>
    public void UpdateTargetingMode()
    {
        if (selectedTower)
        {
            selectedTowerBehaviour.targetingMode = (TowerBehaviour.TargetingMode)targetingModeUI.value;
            UpdateDisplayInformation();
        }
    }

    /// <summary>
    /// Updates the sort order according to the sorting toggle.
    /// </summary>
    public void UpdateSortOrder()
    {
        if (selectedTower)
        {
            selectedTowerBehaviour.sortDescending = sortOrderUI.isOn;
            UpdateDisplayInformation();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void UpdateTooltipUpgradeInformation(int id)
    {
        if (selectedTower && upgradeButtonsUI[id] && upgradeButtonsUI[id].interactable)
        {
            Tooltip.current.ForceShow();

            TowerAttributes attr = selectedTowerBehaviour.attributes + selectedTowerBehaviour.upgradeAttributes;
            TowerAttributes upAttr = selectedTowerBehaviour.GetNextUpgrade(id);
            Tooltip.current.SetText("<b>" + upAttr.displayName + "</b>\n" + TowerAttributes.GetUpgradeTooltip(attr, upAttr));
        }
        else
        {
            Tooltip.current.ForceHide();
        }
    }

    //
    void Start()
    {
        GetComponents();
    }

    void OnEnable()
    {
        UpdateDisplayInformation();
    }

    void OnSelectedTowerChange()
    {
        UpdateDisplayInformation();
    }
    
    bool HasComponents()
    {
        return (displayNameUI != null && targetingModeUI != null && sortOrderUI != null);
    }

    void GetComponents()
    {
        displayNameUI = transform.FindChild("Name").GetComponent<Text>();
        targetingModeUI = transform.FindChild("Dropdown").GetComponent<Dropdown>();
        sortOrderUI = transform.FindChild("Sort Order").GetComponent<Toggle>();
    }
}
