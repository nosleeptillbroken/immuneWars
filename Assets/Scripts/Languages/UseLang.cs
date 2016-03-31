using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Script for accessing LangData through the editor
public class UseLang : MonoBehaviour {

    public string langKey;
    private Text txt;

    // Find the text object to store in txt
    // and set the attached component 
    void Awake()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = LangData.Instance.Retrieve(langKey);
    }

    void OnEnable()
    {
        txt.text = LangData.Instance.Retrieve(langKey);
    }

    public void OnLanguageChange()
    {
        txt.text = LangData.Instance.Retrieve(langKey);
    }
}
