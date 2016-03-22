using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LangData {
    private static LangData instance = null;
    private string[] langPaths = {"../Languages/eng.iwl", "../Languages/frn.iwl"};
    private string langPath = langPaths[0];

    public Dictionary<string, string> current;

    // Loads default language
    private LangData()
    {
        LoadLang();
    }

    public static LangData Instance
    {
        get 
        {
            if (instance == null)
                instance = new LangData();
            return instance;
        }
    }

    private void LoadLang()
    {
        string line;

        // Read the file and display it line by line.
        System.IO.StreamReader file = 
            new System.IO.StreamReader(@langPath);
        while((line = file.ReadLine()) != null)
        {
            Debug.Log(line);
        }

        file.Close();
    }
}
