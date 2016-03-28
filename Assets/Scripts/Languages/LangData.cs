using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LangData {
    private static LangData instance = null;
    private string[] langPaths = {"./Assets/Languages/eng.iwl", "./Assets/Languages/frn.iwl"};
    private string langPath;

    public Dictionary<string, string> current;

    // Loads default language
    private LangData()
    {
        current = new Dictionary<string, string>();
        langPath = langPaths[0];
        LoadLang();
    }

    // ensures one and only one instance of LangData
    public static LangData Instance
    {
        get 
        {
            if (instance == null)
                instance = new LangData();
            return instance;
        }
    }

    // Reads selected language file
    // Splits line entries into keys and values
    // Stores the results in Dictionary current
    private void LoadLang()
    {
        string line;
        string key;
        string value;

        int splitIndex;

        StreamReader file = 
            new StreamReader(@langPath);
        while((line = file.ReadLine()) != null)
        {
            Debug.Log(line);
            splitIndex = line.IndexOf('=', 1);
            Debug.Log(splitIndex);
            if (splitIndex >= 1)
            {
                key = line.Substring(0, splitIndex);
                value = line.Substring(splitIndex + 1, line.Length - (splitIndex + 1));
                Debug.Log(key + "," + value);
                Debug.Log(this.current);
                this.current.Add(key, value);
            }
        }

        file.Close();
    }

    public string Retrieve (string getKey)
    {
        string value;
        if (current.TryGetValue(getKey, out value))
        {
            return value;
        }
        else
        {
            return "Not Found";
        }
    }

    /*
    public string Retrieve (string getKey, string[] insertStrings)
    {
        string value;
        string tempReturn;
        bool flag = false;

        if (insertStrings.Length == 0)
        {
            return Retrieve(getKey);
        }

        if (current.TryGetValue(getKey, out tempReturn))
        {
            int startIndex;
            int endIndex;
            int insertIndex = -1;

            for (int i = 0; i < tempReturn.Length-1; i++)
            {
                if (tempReturn[i] == '$')
                {
                    flag = true;
                    startIndex = i;
                    endIndex = tempReturn.IndexOf(' ', i);

                    if (((insertIndex = tempReturn.Substring(startIndex+1, 
                        endIndex - (startIndex+1)) as int) != null) && 
                        insertIndex < insertStrings.Length)
                    {
                        tempReturn.Replace(tempReturn.Substring(startIndex, (endIndex - startIndex)),
                            insertStrings[insertIndex]);
                    }
                }
            }
            if (flag)
            {
                return tempReturn;
            }
            else
            {
                return Retrieve(getKey);
            }

        }
        else
        {
            return "Not Found";
        }
    }
    */

    public void ChangeLang (string newPath)
    {
        foreach (string path in langPaths)
        {
            if (path.Contains(newPath + ".iwl"))
            {
                langPath = path;
                LoadLang();
                StateManager.instance.SetState(StateManager.instance.currentState); // once a reload function has been enabled, 
            }
            else
            {
                Debug.Log("Path to language file not found.");
            }
        }
    }
}