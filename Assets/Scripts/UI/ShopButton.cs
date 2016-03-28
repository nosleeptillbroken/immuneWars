using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopButton : MonoBehaviour
{
    private Button button = null;

    [HideInInspector]
    public GameObject tower = null;
	
    void Start()
    {
        button = GetComponent<Button>();
        if (tower != null)
        {
            transform.FindChild("Name").GetComponent<Text>().text =
                LangData.Instance.Retrieve(tower.GetComponent<TowerBehaviour>().attributes.displayName);
            transform.FindChild("Cost").GetComponent<Text>().text =
                LangData.Instance.Retrieve("cost") + tower.GetComponent<TowerBehaviour>().attributes.cost;
        }
        AddListener();
    }

    void Update()
    {
        if(Player.instance.currentGold >= tower.GetComponent<TowerBehaviour>().attributes.cost)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    void AddListener()
    {
        UnityEngine.Events.UnityAction selectTower = () =>
        {
            if (TowerManager.instance.selectedTower != null)
            {
                Player.instance.AddGold(TowerManager.instance.selectedTower.GetComponent<TowerBehaviour>().attributes.cost);
                TowerManager.instance.DeselectTower();
            }
            if (Player.instance.RemoveGold(tower.GetComponent<TowerBehaviour>().attributes.cost))
            {
                TowerManager.instance.SelectTower(tower);
            }
        };

        GetComponent<Button>().onClick.AddListener(selectTower);
    }

    void OnTooltip()
    {
        Tooltip.current.ForceShow();
        TowerAttributes attr = tower.GetComponent<TowerBehaviour>().attributes;
        Tooltip.current.SetText("<b>" + attr.displayName + "</b>\n" + attr.description + "\n" + "<color=orange>Cost: " + attr.cost + "</color>");
    }

}
