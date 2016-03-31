using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectionPanel : MonoBehaviour
{

    public GameObject selectedLevel { get { return TowerManager.instance.selectedTower; } }
    public LevelData selectedLevelData { get { return selectedLevel ? selectedLevel.GetComponent<LevelData>() : null; } }

    private Text displayNameUI;
    private Text difficultyUI;

    public Button[] upgradeButtonsUI;

    /// <summary>
    /// Updates the information displayed on the selection panel with the information from the tower and its attributes.
    /// </summary>
    public void UpdateDisplayInformation()
    {
        if (!HasComponents())
        {
            GetComponents();
        }

        if (selectedLevel && selectedLevelData)
        {
            displayNameUI.text = LangData.Instance.Retrieve(selectedLevelData.nameKey);
            difficultyUI.text = LangData.Instance.Retrieve("difficulty") + LangData.Instance.Retrieve(selectedLevelData.difficulty.ToString());
        }
    }

    //
    void Start()
    {
        GetComponents();
    }

    //
    void OnEnable()
    {
        UpdateDisplayInformation();
    }

    //
    void OnSelectedTowerChange()
    {
        UpdateDisplayInformation();
    }

    //
    bool HasComponents()
    {
        return (displayNameUI != null && difficultyUI != null);
    }

    //
    void GetComponents()
    {
        displayNameUI = transform.FindChild("Name").GetComponent<Text>();
        difficultyUI = transform.FindChild("Difficulty").GetComponent<Text>();
    }

    public void StartLevel()
    {
        if (selectedLevel && selectedLevelData)
        {
            StateManager.instance.SetState(StateManager.GameState.InGame, selectedLevelData.level);
        }
    }

    public void CancelLevel()
    {
        TowerManager.instance.DeselectTower();
    }
}
