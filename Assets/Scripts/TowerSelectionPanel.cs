using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerSelectionPanel : MonoBehaviour
{

    public TowerBehaviour selectedTower { get { return TowerSelector.current.selectedTower; } }

    private Text displayNameUI;
    private Dropdown targetingModeUI;
    private Toggle sortOrderUI;

    public Button[] upgradeButtonsUI;

    /// <summary>
    /// Destroys the selected tower without selling it.
    /// </summary>
    public void DestroySelectedTower()
    {
        if(TowerSelector.current && selectedTower)
        {
            Destroy(selectedTower.gameObject);
            TowerSelector.current.DeselectTower();
        }
    }

    /// <summary>
    /// Sells the selected tower.
    /// </summary>
    public void SellSelectedTower()
    {
        if(TowerSelector.current)
        {
            Player.current.AddGold(selectedTower.attributes.cost + selectedTower.upgradeAttributes.cost);
            DestroySelectedTower();
        }
    }

    public void UpgradeSelectedTower(int path)
    {
        if(TowerSelector.current && selectedTower && Player.current.RemoveGold(selectedTower.attributes.cost))
        {
            selectedTower.Upgrade(path);
        }
    }

    /// <summary>
    /// Updates the information displayed on the selection panel with the information from the tower and its attributes.
    /// </summary>
    public void UpdateDisplayInformation()
    {
        if(!HasComponents())
        {
            GetComponents();
        }

        TowerAttributes compositeAttributes = selectedTower.attributes + selectedTower.upgradeAttributes;
        
        displayNameUI.text = compositeAttributes.displayName;

        targetingModeUI.value = (int)selectedTower.targetingMode;

        sortOrderUI.gameObject.GetComponentInChildren<Text>().text = (selectedTower.sortOrder == 1) ? "<" : ">";
        int i = 0;
        foreach (Button btn in upgradeButtonsUI)
        {
            if (selectedTower.CanUpgrade(i))
            {
                btn.transform.FindChild("Name").GetComponent<Text>().text = selectedTower.GetNextUpgrade(i).displayName;
                btn.transform.FindChild("Cost").gameObject.SetActive(true);
                btn.transform.FindChild("Cost").GetComponent<Text>().text = "Cost: " + selectedTower.GetNextUpgrade(i).cost;
                if (Player.current.currentGold >= selectedTower.GetNextUpgrade(i).cost)
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

    /// <summary>
    /// Updates the information for the tower and its attributes using the information from the selection panel.
    /// </summary>
    public void UpdateTowerInformation()
    {
        selectedTower.targetingMode = (TowerBehaviour.TargetingMode)targetingModeUI.value;
        selectedTower.sortOrder = sortOrderUI.isOn ? 1 : -1;
    }


    public void UpdateTooltipUpgradeInformation(int id)
    {
        if (upgradeButtonsUI[id] && upgradeButtonsUI[id].interactable)
        {
            Tooltip.current.ForceShow();

            TowerAttributes attr = selectedTower.attributes + selectedTower.upgradeAttributes;
            TowerAttributes upAttr = selectedTower.GetNextUpgrade(id);
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

    //
    void Update()
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
