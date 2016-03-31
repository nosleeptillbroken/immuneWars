using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestLangData : MonoBehaviour {
    private Text txt;

    void Start()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = LangData.Instance.Retrieve("towerNameIcer");
    }
}
