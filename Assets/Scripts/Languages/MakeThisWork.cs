using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class MakeThisWork : MonoBehaviour {

    private Dropdown menu;
    private List<string> keys = new List<string>();

    void Start()
    {
        menu = gameObject.GetComponent<Dropdown>();
        for (int i = 0; i < menu.options.Count; i++)
        {
            keys.Add(menu.options[i].text);
            menu.options[i].text = LangData.Instance.Retrieve(menu.options[i].text);
        }
    }

    void OnEnable()
    {
        Debug.Log("Dropdown just got enabled.");
        for (int i = 0; i < menu.options.Count; i++)
        {
            Debug.Log("Key = " + keys[i] +
                ", Value = " + LangData.Instance.Retrieve(keys[i]));
            menu.options[i].text = LangData.Instance.Retrieve(keys[i]);
        }
    }

    public void OnLanguageChange()
    {
        Debug.Log("Language Changed");
        for (int i = 0; i < menu.options.Count; i++)
        {
            Debug.Log("Key = " + keys[i] +
                ", Value = " + LangData.Instance.Retrieve(keys[i]));
            menu.options[i].text = LangData.Instance.Retrieve(keys[i]);
        }
    }
}