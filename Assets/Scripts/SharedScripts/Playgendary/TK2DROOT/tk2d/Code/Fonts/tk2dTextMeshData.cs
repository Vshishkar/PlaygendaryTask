using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




[System.Serializable]
public class tk2dTextMeshData
{
	public enum LineBreakMode
	{
		ScaleDown = 0,		// scales label to meet the dimentions
		LineBreak,			// breaks text on lines each one will meet dimentions
		ThreeDots,			// cuts the text at the end and add '...'
		DoNothing,			// just do nothing :)
	}

	[SerializeField] private tk2dFontData font;
	[SerializeField] private string text = "";
	[SerializeField] private Vector3 scale = Vector3.one; 
	[SerializeField] private float spacing = 0.0f;
	private float maxTextLength;
    private float maxTextHeight;
	[SerializeField] private LineBreakMode breakMode;
	[SerializeField] private float lineSpacing = 0.0f;
	[SerializeField] private bool disableNumberGrouping;
    [SerializeField] private bool useShortNumbers;

	[SerializeField] public Color color = Color.white; 
	[SerializeField] public Color color2 = Color.white;
	[SerializeField] public Color colorS = Color.white;
	[SerializeField] public bool useGradient = false; 
	[SerializeField] public int textureGradient = 0;
    [SerializeField] public TextAnchor anchor = TextAnchor.MiddleCenter; 
	[SerializeField] public int renderLayer = 0;
	[SerializeField] public bool kerning = false;  

	[SerializeField] public int prewarmedCharactersCount = 0;

    [SerializeField] bool shouldScaleInCellSize = false;

	private Vector3 lineBreakScale = Vector3.one;


	#region Formatted Text Cache
	private tk2dColoredText formattedTextCache;

	public tk2dFontData Font
	{
		get { return font; }
		set { font = value; formattedTextCache = null; }
	}

	public string Text
	{
		get { return text; }
		set	
		{
			if (text != value)
			{
				text = value;
				formattedTextCache = null;
			}
		}
	}

	public Vector3 UserScale
	{
		get { return scale; }
		set { if (scale != value) { scale = value; formattedTextCache = null; } }
	}


	public float LineSpacing
	{
		set
		{
			lineSpacing = value;
		}
		get
		{
			return lineSpacing;
		}
	}


	public int TextureGradient
	{
		get { return textureGradient; }
		set { textureGradient = value % Font.gradientCount; }
	}

	public float Spacing
	{
        get { return spacing * (tk2dSystem.IsRetina ? 2 : 1); }
		set { if (spacing != value) { spacing = value; formattedTextCache = null; } }
	}
	
	public float MaxTextLength
	{
		get { return maxTextLength; }
		set { if (maxTextLength != value) { maxTextLength = value; formattedTextCache = null; } }
	}

    public float MaxTextHeight
    {
        get { return maxTextHeight; }
        set { if (maxTextHeight != value) { maxTextHeight = value; formattedTextCache = null; } }
    }

	public LineBreakMode BreakMode
	{
		get { return breakMode; }
		set { if (breakMode != value) { breakMode = value; formattedTextCache = null; } }
	}

    public bool ShouldScaleInCellSize
    {
        get { return shouldScaleInCellSize; }
        set { if (shouldScaleInCellSize != value) { shouldScaleInCellSize = value; formattedTextCache = null; } }
    }

	public bool DisableNumberGrouping
	{
		get { return disableNumberGrouping; }
		set { if (disableNumberGrouping != value) { disableNumberGrouping = value; formattedTextCache = null; } }
	}

    public bool UseShortNumbers
    {
        get { return useShortNumbers; }
		set { if (useShortNumbers != value) { useShortNumbers = value; formattedTextCache = null; } }
    }
	#endregion





	public tk2dFontData FontInst
	{
		get
		{
			if (Font != null)
			{
				return font.inst;
			}
			else
			{
				return null;
			}
		}
	}


	public tk2dColoredText FormattedText
	{
		get
		{
			if (formattedTextCache == null)
			{
				formattedTextCache = FormatTextImpl(text, this);
			}

			return formattedTextCache;
		}
	}
		

	
	/// <summary>
	/// Returns the number of characters excluding texture gradient escape codes.
	/// </summary>
	public int VisibleCharactersCount
	{
		get
		{				
			int numChars = 0;
			for (int i = 0; i < FormattedText.Length; ++i)
			{
				int idx = FormattedText[i];
				
				if (FontInst.useDictionary)
				{
					if (!FontInst.CharDict.ContainsKey(idx)) idx = 0;
					
				}
				else
				{
					if (idx >= FontInst.chars.Length) idx = 0; // should be space
				}

				if (idx == '\n')
				{
					continue;
				}
				
				++numChars;
			}
			return numChars;
		}
	}


	public int RequiredCharactersCount
	{
		get
		{
			return Math.Max(VisibleCharactersCount, prewarmedCharactersCount);
		}
	}



	public Bounds GetEstimatedMeshBoundsForString(tk2dColoredText formattedText)
	{
		Vector2 dims = this.GetMeshDimensionsForString(formattedText);
		float offsetY = this.GetYAnchorForHeight(dims.y);
		float offsetX = this.GetXAnchorForWidth(dims.x);
		float lineHeight = (FontInst.lineHeight + lineSpacing) * TotalScale.y;
		return new Bounds( new Vector3(offsetX + dims.x * 0.5f, offsetY + dims.y * 0.5f + lineHeight, 0), Vector3.Scale(dims, new Vector3(1, -1, 1)) );
	}


	public Bounds GetEstimatedMeshBoundsForString(string str)
	{
		return GetEstimatedMeshBoundsForString(new tk2dColoredText(str, Color.white));
	}


	public Bounds EstimatedMeshBounds 
	{
		get
		{
			return GetEstimatedMeshBoundsForString(FormattedText);
		}
	}


	public float ActualLineSpaceHeight
	{
		get
		{
			return ActualLineHeight * (1.0f + lineSpacing / 100.0f);
		}
	}


	public float ActualLineHeight
	{
		get
		{
			return FontInst.lineHeight * TotalScale.y;
		}
	}


	public Vector3 TotalScale
	{
		get 
		{
			Vector3 result = UserScale;
			result.Scale(lineBreakScale);
			return result; 
		}
	}



	#region Private

	// this function is static to ensure that no fields are changed during this call
	private static tk2dColoredText FormatTextImpl(string txt, tk2dTextMeshData meshData)
	{
		string localizedString = LocalisationManager.LocalizedStringOrSource(txt);
		tk2dColoredText result = new tk2dColoredText(localizedString, meshData.colorS);
		ConvertString(result, meshData);
		
		return result;
	}



	// this function is static to ensure that no fields are changed during this call
	private static void ConvertString(tk2dColoredText formattedText, 
	                                  tk2dTextMeshData meshData)
	{
		const char NonBreakableSpace = (char)17;


		if (meshData.UseShortNumbers)
		{       
            formattedText.ApplyShortNumber();			
		}

		if (!meshData.DisableNumberGrouping)
		{
			int currentNumberCount = 0;
			
			for (int i = formattedText.Length - 1; i >= 0; i--)
			{
				var curChar = formattedText[i];
				if (curChar >= '0' && curChar <= '9')
				{
					currentNumberCount += 1;
				}
				else
				{
					currentNumberCount = 0;
				}
				
				if (currentNumberCount == 4)
				{
					// add device control char -> if you add normal space -> 
					// code lower will make new lines
					formattedText.Insert(i + 1, new string(NonBreakableSpace, 1));
					currentNumberCount = 1;
				}
			}
		}


		// reset line break scale before calculate the dimentions
		meshData.lineBreakScale = Vector3.one;
		Vector2 dims = meshData.GetMeshDimensionsForString(formattedText);			

		if (dims.x > meshData.maxTextLength)
		{
			if (meshData.maxTextLength > 0.0f)
			{
				switch (meshData.BreakMode)
				{
				case LineBreakMode.ScaleDown:
				{
					float curScale = meshData.maxTextLength / dims.x;
					meshData.lineBreakScale.Set(curScale,
					                            curScale,
					                            curScale);
				}
					break;


				case LineBreakMode.ThreeDots:
				{
					formattedText.Append("...");
					for (int i = formattedText.Length - 4; i >= 0 && meshData.GetMeshDimensionsForString(formattedText).x > meshData.MaxTextLength; i--)
					{
						formattedText.Remove(i);
					}
				}
					break;


				case LineBreakMode.LineBreak:
				{
					int curStartIndex = 0;
					int curFinishIndex = 0;
					while (curFinishIndex < (formattedText.Length - 1))
					{
						int curNextSpace = formattedText.IndexOf(' ', curFinishIndex + 1);
						int curLastIndex = (curNextSpace != -1) ? curNextSpace : (formattedText.Length - 1);
						if (meshData.GetMeshDimensionsForString(formattedText, curStartIndex, (curLastIndex - curStartIndex + 1)).x > meshData.maxTextLength)
						{
							int actualFinishIndex = (curFinishIndex > curStartIndex) ? curFinishIndex : curLastIndex;
							
							formattedText[actualFinishIndex] = '\n';
							
							curFinishIndex = curStartIndex = actualFinishIndex + 1;
						}
						else
						{
							curFinishIndex = curLastIndex;
						}
					}
				}
					break;
				}
			}
		}

        float dimsY = Mathf.Abs(meshData.GetMeshDimensionsForString(formattedText).y);
        if (dimsY > meshData.maxTextHeight)
        {
            if (meshData.maxTextHeight > 0.0f)
            {
                float curScale = meshData.lineBreakScale.x;
                float newScale = meshData.maxTextHeight / dimsY;
                curScale = Math.Min(curScale, newScale);
                meshData.lineBreakScale.Set(curScale,
                    curScale,
                    curScale);
            }
        }

        if (meshData.ShouldScaleInCellSize)
        {
            float newScale = Mathf.Min(meshData.maxTextLength / dims.x, meshData.MaxTextHeight / Mathf.Abs(dims.y));
            meshData.lineBreakScale.Set(newScale, newScale, newScale);
        }

		formattedText.Replace(NonBreakableSpace, ' ');
	}

	#endregion
}

