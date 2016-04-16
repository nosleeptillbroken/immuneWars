using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DropdownLang : MonoBehaviour
{
    Dropdown dropdown = null;
    public string[] keys;

    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    void Update()
    {
        if (dropdown)
        {
            dropdown.captionText.text = LangData.Instance.Retrieve(keys[dropdown.value]);
            int i = 0;
            foreach (var option in dropdown.options)
            {
                if (i < dropdown.options.Count)
                {
                    option.text = LangData.Instance.Retrieve(keys[i]);
                    i++;
                }
            }
        }
    }

}
