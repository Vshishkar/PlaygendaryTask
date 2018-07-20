using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class tk2dColoredText
{
	#region Color Command

	abstract class ColorCommand
	{
		public abstract void Apply(tk2dTextGeomGen.InlineStyler styler);

		public static ColorCommand Parse(string input, out int endPosition)
		{
			ColorCommand result = null;
			int cmdLength = 0;

			if (input.Length >= 1)
			{
				var cmdSymbol = input[0];

				// try to handle texU command
				if (cmdSymbol >= '0' && cmdSymbol <= '9')
				{
					result = SetTexUCommand.Parse(cmdSymbol);
					cmdLength = 1;
				}
				else
				{
					string cmdArgs = input.Substring(1);

					int setColorLength = 0;
					switch (cmdSymbol)
					{
					case 'c': result = SetColorCommand.Parse(cmdArgs, false, false, out setColorLength); break;
					case 'C': result = SetColorCommand.Parse(cmdArgs, false, true, out setColorLength); break;
					case 'g': result = SetColorCommand.Parse(cmdArgs, true, false, out setColorLength); break;
					case 'G': result = SetColorCommand.Parse(cmdArgs, true, true, out setColorLength); break;
					}

					cmdLength = 1 + setColorLength;
				}
			}

			if (result != null)
			{
				endPosition = cmdLength;
			}
			else
			{
				endPosition = 0;
			}

			return result;
		}
	}




	class SetColorCommand : ColorCommand
	{
		static readonly Color32 ErrorColor = new Color32(255, 0, 255, 255);

		Color32 topColor;
		Color32 bottomColor;

		public SetColorCommand(Color32 _color)
		{
			topColor = bottomColor = _color;
		}


		public SetColorCommand(Color32 _topColor, Color32 _bottomColor)
		{
			topColor = _topColor;
			bottomColor = _bottomColor;
		}


		public override void Apply(tk2dTextGeomGen.InlineStyler styler)
		{
			styler.meshTopColor = topColor;
			styler.meshBottomColor = bottomColor;
		}


		public static SetColorCommand Parse(string input, bool twoColors, bool fullHex, out int endPosition)
		{
			int argLength = (twoColors ? 2 : 1) * (fullHex ? 8 : 4);
			bool error = false;

			Color32 topColor = ErrorColor;
			Color32 bottomColor = ErrorColor;

			if (input.Length >= argLength) 
			{
				if (!TryGetStyleHexColor(input, fullHex, out topColor)) 
				{
					error = true;
				}
				
				if (twoColors) 
				{
					string color2 = input.Substring (fullHex ? 8 : 4);
					if (!TryGetStyleHexColor(color2, fullHex, out bottomColor)) 
					{
						error = true;
					}
				}
				else
				{
					bottomColor = topColor;
				}
			}
			else
			{
				error = true;
			}

			endPosition = argLength;
			if (error) 
			{
				return new SetColorCommand(ErrorColor);
			}
			else
			{
				return new SetColorCommand(topColor, bottomColor);
			}
		}


		static bool TryGetStyleHexColor(string str, bool fullHex, out Color32 color)
		{
			color = ErrorColor;

			int r, g, b, a;
			if (fullHex)
			{
				if (str.Length < 8) return false;
				r = GetFullHexColorComponent(str[0], str[1]);
				g = GetFullHexColorComponent(str[2], str[3]);
				b = GetFullHexColorComponent(str[4], str[5]);
				a = GetFullHexColorComponent(str[6], str[7]);
			} 
			else 
			{
				if (str.Length < 4) return false;
				r = GetCompactHexColorComponent(str[0]);
				g = GetCompactHexColorComponent(str[1]);
				b = GetCompactHexColorComponent(str[2]);
				a = GetCompactHexColorComponent(str[3]);
			}
			
			if (r == -1 || g == -1 || b == -1 || a == -1) 
			{
				return false;
			}
			
			color = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
			return true;
		}


		static int GetFullHexColorComponent(int c1, int c2) 
		{
			int result = 0;
			if (c1 >= '0' && c1 <= '9') result += (c1 - '0') * 16;
			else if (c1 >= 'a' && c1 <= 'f') result += (10 + c1 - 'a') * 16;
			else if (c1 >= 'A' && c1 <= 'F') result += (10 + c1 - 'A') * 16;
			else return -1;
			if (c2 >= '0' && c2 <= '9') result += (c2 - '0');
			else if (c2 >= 'a' && c2 <= 'f') result += (10 + c2 - 'a');
			else if (c2 >= 'A' && c2 <= 'F') result += (10 + c2 - 'A');
			else return -1;
			return result;
		}


		static int GetCompactHexColorComponent(int c) 
		{
			if (c >= '0' && c <= '9') return (c - '0') * 17;
			if (c >= 'a' && c <= 'f') return (10 + c - 'a') * 17;
			if (c >= 'A' && c <= 'F') return (10 + c - 'A') * 17;
			return -1;
		}
	}




	class SetTexUCommand : ColorCommand
	{
		float texU;

		public SetTexUCommand(float _texU)
		{
			texU = _texU;
		}


		public override void Apply(tk2dTextGeomGen.InlineStyler styler)
		{
			styler.meshGradientTexU = texU / (float)((styler.curGradientCount > 0) ? styler.curGradientCount : 1);
		}


		public static SetTexUCommand Parse(char inputSymbol)
		{
			return new SetTexUCommand((float)(inputSymbol - '0'));
		}
	}

	#endregion


	#region Color Command Set
	
	class ColorCommandSet
	{
		Dictionary<int, ColorCommand> colorCommands = null;
		Dictionary<int, ColorCommand> ColorCommands
		{
			get
			{
				if (colorCommands == null)
				{
					colorCommands = new Dictionary<int, ColorCommand>();
				}

				return colorCommands;
			}
		}


		public ColorCommand GetCommandAtIndex(int index)
		{
			// access through field to prevent unnecessary 'new'
			if (colorCommands != null && ColorCommands.ContainsKey(index))
			{
				return ColorCommands[index];
			}
			else
			{
				return null;
			}
		}


		public void MoveCommandsBy(int startIndex, int offset)
		{
			// access through field to prevent unnecessary 'new'
			if (colorCommands != null)
			{
				Dictionary<int, ColorCommand> newDictionary = new Dictionary<int, ColorCommand>(ColorCommands.Count);

				foreach (var curPair in ColorCommands)
				{
					newDictionary.Add(curPair.Key + (curPair.Key >= startIndex ? offset : 0), curPair.Value);
				}

				colorCommands = newDictionary;
			}
		}


		public void Add(int index, ColorCommand command)
		{
			ColorCommands.Remove(index);
			ColorCommands.Add(index, command);
		}


		public void Remove(int index, int count)
		{
			for (int i = index; i < (index + count); i++)
			{
				ColorCommands.Remove(i);
			}
		}
	}
	
	#endregion


	#region Statics

	static string rgb2fmtclr(string[] values)
	{
		int r = Int32.Parse(values[0]);
		int g = Int32.Parse(values[1]);
		int b = Int32.Parse(values[2]);
		
		return String.Format("^C{0}{1}{2}FF", r.ToString("X2"), g.ToString("X2"), b.ToString("X2"));
	}


	static string hex2fmtclr(string[] values)
	{
		return String.Format("^C{0}{1}{2}FF", values[0], values[1], values[2]);
	}


	static string flt2fmtclr(string[] values)
	{
		int r = (int)(Double.Parse(values[0]) * 255);
		int g = (int)(Double.Parse(values[1]) * 255);
		int b = (int)(Double.Parse(values[2]) * 255);
		
		string[] rgbValues = { r.ToString("d"), g.ToString("d"), b.ToString("d") };
		
		return rgb2fmtclr(rgbValues);
	}


	enum ColorType : byte {unk = 0, rgb, hex, flt};

	static string ParceColor(string color)
	{
		ColorType cType = ColorType.unk;
		
		// detect color type
		int tStart = color.IndexOf("=") + 1;
		int tEnd = color.IndexOf("(");
		string tString = color.Substring(tStart, tEnd - tStart);
		
		if(tString == "rgb")
		{
			cType = ColorType.rgb;
		}else
			if(tString == "hex")
		{
			cType = ColorType.hex;
		}else
			if(tString == "flt")
		{
			cType = ColorType.flt;
		}
		
		int vStart = tEnd + 1, vEnd = color.IndexOf(")", vStart);
		string[] values = color.Substring(vStart, vEnd - vStart).Split(';');
		string result = String.Empty;
		
		switch(cType) 
		{
		case ColorType.rgb:
			result = rgb2fmtclr(values);
			break;
		case ColorType.hex:
			result = hex2fmtclr(values);
			break;
		case ColorType.flt:
			result = flt2fmtclr(values);
			break;
		}
		
		return result;
	}


	static string Colorize(string formattedString, Color defaultColor)
	{
		string[] stringItems = formattedString.Split(new char[] {'<', '>'});
		
		if(stringItems.Length <= 1)
		{
			return formattedString;
		}
		
		string[] defaultValues = {	
			defaultColor.r.ToString("f"),
			defaultColor.g.ToString("f"), 
			defaultColor.b.ToString("f") 
		};
		string defaultString = flt2fmtclr(defaultValues);
		string resultString = defaultString; //starts with default color
		
		for (int idx = 0; idx < stringItems.Length; idx++) 
		{
			string item = stringItems[idx];
			
			if(item.StartsWith("color=")) 
			{
				resultString += ParceColor(item);
			}
			else if (item.Equals("/color"))
			{
				resultString += defaultString;
			}
			else
			{
				resultString += item;
			}
		}
		
		return resultString;
	}
	

	static string RemoveColorCommands(string formattedString, out ColorCommandSet colorCommandSet)
	{
		colorCommandSet = new ColorCommandSet();

		System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder(formattedString.Length);

		for (int i = 0; i < formattedString.Length; ++i)
		{	
			var charIndex = formattedString[i];
			if (charIndex == '^')
			{
				if (i + 1 < formattedString.Length)
				{
					if (formattedString[i + 1] == '^') 
					{
						resultBuilder.Append('^');
						i += 1;
					}
					else 
					{
						int cmdLength = 0;
						ColorCommand curCommand = ColorCommand.Parse(formattedString.Substring(i + 1), out cmdLength);

						if (curCommand != null)
						{
							colorCommandSet.Add(resultBuilder.Length, curCommand);
							i += cmdLength;
						}
					}
				}
			}
			else
			{
				resultBuilder.Append(charIndex);
			}
		}

		return resultBuilder.ToString();
	}

	#endregion


	#region Variables
	
    StringBuilder text;
	ColorCommandSet commandSet;
	
	#endregion


	#region Properties

	public int Length
	{
		get { return text.Length; }
	}


	public char this[int idx]
	{
		get
		{
			return text[idx];
		}
		set
		{
			text = text.Remove(idx, 1);
			text = text.Insert(idx, new string(value, 1));
		}
	}

	#endregion


	#region Contructors
	
	public tk2dColoredText(string source, Color defaultColor)
	{
        text = new StringBuilder(RemoveColorCommands(Colorize(source, defaultColor), out commandSet));
	}

	#endregion


	#region Public
	
	public void ApplyColorCommand(tk2dTextGeomGen.InlineStyler styler, int index)
	{
		var curCommand = commandSet.GetCommandAtIndex(index);
		if (curCommand != null)
		{
			curCommand.Apply(styler);
		}
	}


	public void Insert(int idx, string toInsert)
	{
		text = text.Insert(idx, toInsert);
		commandSet.MoveCommandsBy(idx, toInsert.Length);
	}


	public void Remove(int startIdx, int count = 1)
	{
		text = text.Remove(startIdx, count);
		commandSet.Remove(startIdx, count);
		commandSet.MoveCommandsBy(startIdx + count, -count);
	}


	public void Append(string strToAppend)
	{
        text.Append(strToAppend);
	}


	public void Replace(char oldChar, char newChar)
	{
		text = text.Replace(oldChar, newChar);
	}


	public int IndexOf(char charToFind)
	{
        return text.ToString().IndexOf(charToFind);
	}


	public int IndexOf(char charToFind, int startIdx)
	{
        return text.ToString().IndexOf(charToFind, startIdx);
	}

    public void ApplyShortNumber()
    {
        string txt = text.ToString().GetShortNumberForm();

        text.Length = 0;
        text.Append(txt);
    }

    public override string ToString()
    {
        return text.ToString();
    }
	#endregion
}
	
