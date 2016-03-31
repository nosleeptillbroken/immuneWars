using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//function to safely hold language objects
class Language
{
    // data for each language
    private string name;
    private string key;

    // public constructor for each language
    // if things are screwed up with the parameters it defaults to english
    public Language(string newName, string newKey)
    {
        /*if (newName != null)
        {
            name = newName;
        }
        else
        {
            Debug.Log("Do not call with empty name string");
            name = "English";
        }*/
        name = (newName != null) ? newName : "English";
        /*if (newKey != null)
        {
            key = newKey;
        }
        else
        {
            Debug.Log("Do not call with an empty key");
            key = "eng";
        }*/
        key = (newKey != null) ? newKey : "eng";
    }

    // getters so that data is protected and accessible
    public string Name
    {
        get{ return name; }
    }

    public string Key
    {
        get{ return key; }
    }

}

public class LangData {
    private static LangData instance = null;
    private const string path = "./Assets/Languages/";
    private const string ext = ".iwl";
    private List<Language> languages = new List<Language>();
    private int currentLangIndex = 0;


    public Dictionary<string, string> current;

    // Loads default language
    private LangData()
    {
        current = new Dictionary<string, string>();
        PopulateLanguages();
        LoadLang();
        Debug.Log(languages.Count);
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

    private void PopulateLanguages()
    {
        foreach (string fileName in Directory.GetFiles(path, "*.iwl"))
        {
            StreamReader file = 
                new StreamReader(@fileName);
            string langName = file.ReadLine();
            Debug.Log(fileName.Length + ',' + fileName.LastIndexOf('/'));
            Language newLang = new Language(langName, 
                                   fileName.Substring(fileName.LastIndexOf('/') + 1, 3));
            languages.Add(newLang);
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
        string fullPath = path + languages[currentLangIndex].Key + ext;
        int splitIndex;

        StreamReader file = 
            new StreamReader(@fullPath);
        while((line = file.ReadLine()) != null)
        {
            splitIndex = (line.Length >= 3) && line.Contains("=") ? line.IndexOf('=', 1) : 0;
            if (splitIndex >= 1)
            {
                key = line.Substring(0, splitIndex);
                value = line.Substring(splitIndex + 1, line.Length - (splitIndex + 1));
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

    public void ChangeLang (int newLangIndex)
    {
        if ((newLangIndex >= 0) && (newLangIndex <= languages.Count - 1)
            && (newLangIndex != currentLangIndex))
        {
            UseLang[] langGetters = GameObject.FindObjectsOfType<UseLang>();
            ShopButton[] shopButtons = GameObject.FindObjectsOfType<ShopButton>();

            currentLangIndex = newLangIndex;
            current.Clear();
            LoadLang();
            // StateManager.instance.SetState(StateManager.instance.currentState); possibly not using this
            foreach (UseLang textObject in langGetters)
            {
                textObject.SendMessage("OnLanguageChange");
            }

            foreach (ShopButton buttonObject in shopButtons)
            {
                buttonObject.SendMessage("OnLanguageChange");
            }
        }
        else if (newLangIndex == currentLangIndex)
        {
            Debug.Log("That language is already selected.");
        }
        else
        {
            Debug.Log("Path to language file not found.");
            Debug.Log(newLangIndex);

        }
    }
}