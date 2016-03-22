using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UseLang : MonoBehaviour {
    public string langKey;

    private Text txt;

    void Awake()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = LangData.Instance.Retrieve(langKey);
    }
}
