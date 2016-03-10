using UnityEngine;
using System.Collections.Generic;

public class TowerShop : MonoBehaviour {
    
    [Header("Shop")]
    public List<GameObject> towers;

    [Header("Animation")]
    public float animationSpeed = 5.0f;
    Vector3 newPosition;

    [Header("Layout")]
    public int columns = 3;
    public float padding = 8.0f;
    public float buttonWidth = 32.0f;
    private GameObject buttonTemplate = null;
    public List<GameObject> buttons = null;

	void Start ()
    {
        RectTransform rectTransform = (RectTransform)transform;

        buttonTemplate = transform.FindChild("Template").gameObject;

        newPosition = GetComponent<RectTransform>().anchoredPosition;

        int i = 0;

        int row = 0;
        int col = 0;

        float distanceBetweenButtons = buttonWidth + padding;
        float totalPanelWidth = ((columns + 1) * padding) + (columns * buttonWidth);

        rectTransform.sizeDelta = new Vector2(totalPanelWidth-800, rectTransform.sizeDelta.y);

        foreach(GameObject tower in towers)
        {
            row = i / columns;
            col = i % columns;

            GameObject button = Instantiate(buttonTemplate);
            ShopButton shopButton = button.GetComponent<ShopButton>();

            shopButton.tower = tower;

            RectTransform buttonTransform = (RectTransform)button.transform;
            buttonTransform.SetParent(transform, false);

            buttonTransform.sizeDelta = new Vector2(buttonWidth, buttonWidth);
            buttonTransform.anchoredPosition += new Vector2(distanceBetweenButtons * col, -distanceBetweenButtons * row);

            button.SetActive(true);
            buttons.Add(button);

            i++;
        }
	}

    // Called every frame after Update.
    void LateUpdate()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(GetComponent<RectTransform>().anchoredPosition, newPosition, Time.deltaTime * animationSpeed);
    }

    /// <summary>
    /// Opens the shop panel.
    /// </summary>
    public void OpenPanel()
    {
        RectTransform rectTransform = (RectTransform)transform;
        Vector3 delta = new Vector3(rectTransform.rect.width, 0.0f);
        newPosition -= delta;
    }

    /// <summary>
    /// Closes the shop panel.
    /// </summary>
    public void ClosePanel()
    {
        RectTransform rectTransform = (RectTransform)transform;
        Vector3 delta = new Vector3(rectTransform.rect.width, 0.0f);
        newPosition += delta;

        if(TowerManager.instance.selectedTower)
        {
            Player.instance.AddGold(TowerManager.instance.selectedTower.GetComponent<TowerBehaviour>().attributes.cost);
            TowerManager.instance.DeselectTower();
        }

    }
}
