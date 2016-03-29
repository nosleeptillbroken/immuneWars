using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UseLang : MonoBehaviour {
    public string langKey;

    private Text txt;

    void Start()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = LangData.Instance.Retrieve(langKey);
    }

    public void OnLanguageChange()
    {
        txt.text = LangData.Instance.Retrieve(langKey);
    }
}
