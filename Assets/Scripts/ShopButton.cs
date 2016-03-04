using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopButton : MonoBehaviour
{
    
    public GameObject tower = null;
	
    public void Start()
    {
        if (tower != null)
        {
            transform.FindChild("Name").GetComponent<Text>().text = tower.GetComponent<TowerBehaviour>().attributes.displayName;
            transform.FindChild("Cost").GetComponent<Text>().text = "Cost: " + tower.GetComponent<TowerBehaviour>().attributes.cost;
        }
        AddListener();
    }

    void AddListener()
    {
        UnityEngine.Events.UnityAction selectTower = () =>
        {
            TowerSpawner.current.SetSelectedTower(tower);
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
