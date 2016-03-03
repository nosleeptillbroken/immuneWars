using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopButton : MonoBehaviour {

    public TowerSpawner spawner = null;
    public GameObject tower = null;
	
    public void Start()
    {
        if (tower != null)
        {
            transform.FindChild("Name").GetComponent<Text>().text = tower.GetComponent<TowerBehaviour>().attributes.displayName;
            transform.FindChild("Cost").GetComponent<Text>().text = "Cost: " + tower.GetComponent<TowerBehaviour>().attributes.cost;
        }
        if(spawner != null)
        {

        }

        AddListener();
    }

    void AddListener()
    {
        UnityEngine.Events.UnityAction selectTower = () =>
        {
            spawner.SetSelectedTower(tower);
        };

        GetComponent<Button>().onClick.AddListener(selectTower);
    }

}
