using UnityEngine;
using UnityEngine.UI;

public class ChangeLang : MonoBehaviour {
   
    public Dropdown myDropdown;

    void Start() 
    {
        myDropdown.onValueChanged.AddListener(delegate {
            myDropdownValueChangedHandler(myDropdown);
        });
    }
    void Destroy() 
    {
        myDropdown.onValueChanged.RemoveAllListeners();
    }

    private void myDropdownValueChangedHandler(Dropdown target) 
    {
        Debug.Log("selected: "+target.value);
        LangData.Instance.ChangeLang(target.value);
    }

    public void SetDropdownIndex(int index) 
    {
        myDropdown.value = index;
    }
}
