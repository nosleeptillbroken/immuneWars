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
            transform.FindChild("Name").GetComponent<Text>().text = tower.GetComponent<TowerBehaviour>().attributes.displayName;
            transform.FindChild("Cost").GetComponent<Text>().text = "Cost: " + tower.GetComponent<TowerBehaviour>().attributes.cost;
        }
        AddListener();
    }

    void Update()
    {
        if(Player.current.currentGold >= tower.GetComponent<TowerBehaviour>().attributes.cost)
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
            if (Player.current.RemoveGold(tower.GetComponent<TowerBehaviour>().attributes.cost))
            {
                TowerSpawner.current.Selecttower(tower);
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
