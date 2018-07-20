/*
	CSVReader by Dock. (24/8/11)
	http://starfruitgames.com
 
	usage: 
	CSVReader.SplitCsvGrid(textString)
 
	returns a 2D string array. 
 
	Drag onto a gameobject for a demo of CSV parsing.
*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public static class CSVReader 
{
//	public TextAsset csvFile; 
//	public void Start()
//	{
//		string[,] grid = SplitCsvGrid(csvFile.text);
//		DebugOutputGrid(grid); 
//	}

	static string COMMENT_KEY = "[@comment]";

	// outputs the content of a 2D array, useful for checking the importer
	static public void DebugOutputGrid(string[,] grid)
	{
		for (int y = 0; y < grid.GetUpperBound(1); y++) 
		{	
			if(!string.IsNullOrEmpty(grid[0, y]))
			{
				string output = "";
				for (int x = 0; x < grid.GetUpperBound(0); x++) 
				{
					output += grid[x,y] + ", ";
				}

				CustomDebug.Log(output);
			}
		}
	}

	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		string[] lines = SplitCsvGridByRows(csvText); 

		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}

		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		for (int y = 0; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine( lines[y] ); 
			for (int x = 0; x < row.Length; x++) 
			{
				outputGrid[x,y] = row[x]; 

				// This line was to replace "" with " in my output. 
				// Include or edit it as you wish.
//				outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
			}
		}

		return outputGrid; 
	}


	// splits a CSV file into a 2D string array
	static public string[] SplitCsvGridByRows(string csvText)
	{
		string [] splitedText = csvText.Split ('\n');
		List<string> finalRows = new List<string> ();

		foreach(string row in splitedText)
		{
			if(!row.Contains(COMMENT_KEY))
			{
				finalRows.Add(row);
			}
		}

		return finalRows.ToArray();
	}

	static public string GetJsonStringWithCsvFormat(string json)
	{
		return json.Replace ("\"", "\"\"").Replace (",", "\",\"");
	}

	// splits a CSV row 
	static public string[] SplitCsvLine(string line)	
	{
		line = line.Replace("\"\"", "\"");
		
		List<string> lines = new List<string> ();
		
		int startIndex = 0;
		int charsToLine = 0;
		
		for(int i = 0; i < line.Length; i++)
		{
			charsToLine++;
			if((line[i] == ',' && line[i - 1] != '\"') || (i == line.Length - 1))
			{
				lines.Add(line.Substring(startIndex,(i == line.Length - 1) ? charsToLine : charsToLine - 1).Replace("\",\"", ","));
				startIndex = i + 1;
				charsToLine = 0;
			}
		}

		return lines.ToArray();
	}


	// save CSV file
	static public void SaveCSV(Dictionary<string, string> keysData, string path = "Assets/Resources/Texts/Localisation/KeysData.csv")
	{
		FileStream fs = new FileStream (path, FileMode.Create);
		StreamWriter writer = new StreamWriter (fs);
		foreach(KeyValuePair<string, string> pair in keysData)
		{
			writer.WriteLine(pair.Key + "," + pair.Value);
		}
		writer.Close ();

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif

	}

	//save one key
	static public void SaveLineToCSV(string key, string value, string fileName)
	{
		FileStream fs = new FileStream ("Assets/Resources/Texts/" + fileName + ".csv", FileMode.Append);
		StreamWriter writer = new StreamWriter (fs);

		writer.WriteLine(key + "," + value);

		writer.Close ();
		
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
}