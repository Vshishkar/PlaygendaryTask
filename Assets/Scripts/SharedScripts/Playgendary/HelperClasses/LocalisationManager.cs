using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public struct LanguageAsset
{
    public List<SystemLanguage> languages;
    public TextAsset fontsAsset;
    public TextAsset textAsset;
    public string languageName;
}


public class LocalisationManager : SingletonMonoBehaviour<LocalisationManager> 
{
	#region Variables

    public static event System.Action OnLanguageChanged;

	[SerializeField] TextAsset keysFile;

    [SerializeField] LanguageAsset defaultLanguageAsset;
    [SerializeField] List<LanguageAsset> otherLanguagesAssets;

    [SerializeField] List<tk2dFontData> fontsData;

	Dictionary<string, string> internal_allTexts = null;

    LanguageAsset currentLanguageAsset;

    List<tk2dFontData> defaultLanguageFonts = new List<tk2dFontData>();
    List<tk2dFontData> currentLanguageFonts = new List<tk2dFontData>();

    #endregion


    #region Properties

    public string CurrentLanguageAssetLanguageName
    {
        get
        {
            return currentLanguageAsset.languageName;
        }
    }


    public List<SystemLanguage> CurrentLanguageAssetLanguages
    {
        get
        {
            return currentLanguageAsset.languages;
        }
    }


    public List<tk2dFontData> DefaultLanguageFonts
    {
        get
        {
            if (defaultLanguageFonts.Count == 0)
            {
                if (defaultLanguageAsset.languages != null && defaultLanguageAsset.textAsset != null && defaultLanguageAsset.fontsAsset != null)
                {
                    string[] fontNames = CSVReader.SplitCsvGridByRows(defaultLanguageAsset.fontsAsset.text);
                    for (int i = 0; i < fontNames.Length; i++)
                    {
                        for (int j = 0; j < fontsData.Count; j++)
                        {
                            if (fontsData[j].name == string.Format("{0} data", fontNames[i]))
                            {
                                defaultLanguageFonts.Add(fontsData[j]);
                                break;
                            }
                        }
                    }
                }
            }

            return defaultLanguageFonts;
        }
    }


    public List<tk2dFontData> CurrentLanguageFonts
    {
        get
        {
            return currentLanguageFonts;
        }

        private set
        {
            currentLanguageFonts = value;

            int addedFontsCount = DefaultLanguageFonts.Count - currentLanguageFonts.Count;
            if (addedFontsCount > 0)
            {
                tk2dFontData addedFont = currentLanguageFonts[currentLanguageFonts.Count - 1];
                for (int i = 0; i < addedFontsCount; i++)
                {
                    currentLanguageFonts.Add(addedFont);
                }
            }
        }
    }


    TextAsset KeysFile
    {
        get
        {
            return keysFile;
        }

        set
        {
            if (keysFile != value)
            {
                keysFile = value;

                internal_allTexts = null;
            }
        }
    }


	Dictionary<string, string> AllTexts
	{
		get
		{
			if (internal_allTexts == null)
			{
				internal_allTexts = new Dictionary<string, string>();

				string[,] loadedText = CSVReader.SplitCsvGrid(keysFile.text);
				for (int y = 0; y < loadedText.GetUpperBound(1); y++) 
				{	
					if(!string.IsNullOrEmpty(loadedText[0, y]))
					{
						if(internal_allTexts.ContainsKey(loadedText[0,y]))
						{
							CustomDebug.LogError("KEY ALLREADY EXISTS = " + loadedText[0,y]);
						}
						else
						{
							string value = loadedText[1,y];
							
							if (!string.IsNullOrEmpty(value))
							{
								value = value.Replace(Constants.LocalizationTags.LINE, "\n");
                                value = value.Replace(Constants.LocalizationTags.COMMA, ",");
								
								internal_allTexts.Add(loadedText[0,y], value);
							}
							else
							{
                                CustomDebug.LogWarning("Null ref in key : " + loadedText[0,y]);
							}
						}
					}
				}
			}

			return internal_allTexts;
		}
	}

	#endregion


    #region Unity lifecycle     

    protected override void Awake()
    {
        currentLanguageAsset = defaultLanguageAsset;
        currentLanguageFonts = DefaultLanguageFonts;

        base.Awake();
    }

    #endregion

	
	#region Public methods

	public string GetTextByKey(string key)
	{
        string result = null;

        if (!AllTexts.TryGetValue(key, out result))
		{
            result = key;
		}

		return result;
	}


	public static string LocalizedStringOrSource(string source)
	{
		string result = source;

		if (InstanceIfExist)
		{
			string loadedText = Instance.GetTextByKey(result);
			if (loadedText != null)
			{
				result = loadedText;
			}
		}

		return result;
	}


    public void ChangeLanguage(SystemLanguage currentLanguage)
    {
        LanguageAsset nextLanguageAsset = defaultLanguageAsset;

        if (!defaultLanguageAsset.languages.Contains(currentLanguage))
        {
            for (int i = 0; i < otherLanguagesAssets.Count; i++)
            {
                if (otherLanguagesAssets[i].languages.Contains(currentLanguage))
                {
                    nextLanguageAsset = otherLanguagesAssets[i];
                    break;
                }
            }
        }
            
        if (currentLanguageAsset.languages != nextLanguageAsset.languages)
        {
            currentLanguageAsset = nextLanguageAsset;

            KeysFile = nextLanguageAsset.textAsset;

            string[] nextFonts;

            if (nextLanguageAsset.fontsAsset != null)
            {
                nextFonts = CSVReader.SplitCsvGridByRows(nextLanguageAsset.fontsAsset.text);
                List<tk2dFontData> nextFontsData = new List<tk2dFontData>();
                for (int i = 0; i < nextFonts.Length; i++)
                {
                    for (int j = 0; j < fontsData.Count; j++)
                    {
                        if (fontsData[j].name == string.Format("{0} data", nextFonts[i]))
                        {
                            nextFontsData.Add(fontsData[j]);
                            break;
                        }
                    }
                }
                CurrentLanguageFonts = new List<tk2dFontData>(nextFontsData);
            }

            if (OnLanguageChanged != null)
            {
                OnLanguageChanged();
            }
        }
    }

	#endregion
}