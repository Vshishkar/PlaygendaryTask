/*
 * Copyright (c) 2013 Calvin Rien
 *
* Based on the JSON parser by Patrick van Bergen
* http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
*
* Simplified it so that it doesn't throw exceptions
* and can be used in Unity iPhone with maximum code stripping.
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
	* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
	* the following conditions:
	*
	* The above copyright notice and this permission notice shall be
	* included in all copies or substantial portions of the Software.
	*
	* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
	* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
	* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
	*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace MiniJSON 
{
	public class JSONParser : IDisposable 
	{
		const string WORD_BREAK = "{}[],:\"";

		static Type objectType = typeof(System.Object);
		static Type dictionaryType = typeof(Dictionary<,>);
		static Type defaultDictionaryType = typeof(Dictionary<string, object>);


		public static bool IsWordBreak(char c) {
			return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
		}


		public enum TOKEN {
			NONE,
			CURLY_OPEN,
			CURLY_CLOSE,
			SQUARED_OPEN,
			SQUARED_CLOSE,
			COLON,
			COMMA,
			STRING,
			NUMBER,
			TRUE,
			FALSE,
			NULL
		};

		StringReader json;


		JSONParser(string jsonString) 
		{
			json = new StringReader(jsonString);
		}


		public static object Parse(string jsonString, Type type) 
		{
			using (var instance = new JSONParser(jsonString))
			{
				return instance.ParseValue(type);
			}
		}


		public void Dispose() {
			json.Dispose();
			json = null;
		}


		public object ParseObject(Type type) 
		{
			if(type.IsGenericType && type.GetGenericTypeDefinition() == dictionaryType)
			{
				return ParseDictionary(type);
			}

			if(type.Equals(objectType))
			{
				return ParseDictionary(defaultDictionaryType);
			}

			return ParseClass(type);
		}


		public object ParseDictionary(Type type) 
		{
			IDictionary table = Activator.CreateInstance(type) as IDictionary;
			Type keyType = type.GetGenericArguments()[0];
			Type valueType = type.GetGenericArguments()[1];

			// ditch opening brace
			json.Read();

			// {
			while (true) {
				switch (NextToken) {
					case TOKEN.NONE:
						return null;
					case TOKEN.COMMA:
						continue;
					case TOKEN.CURLY_CLOSE:
						if(table.Contains("__type"))
						{
							string typeString = table["__type"] as string;
							if (typeString != null) 
							{
								if(Json.JsonConvertersString.ContainsKey(typeString))
								{
									return Json.JsonConvertersString[typeString].Deserialize(table);
								}
							}
						}
						return table;
					default:
						// name
						object name = ParseValue(keyType);
						if (name == null) {
							return null;
						}

						// :
						if (NextToken != TOKEN.COLON) {
							return null;
						}
						// ditch the colon
						json.Read();

						// value
						object val = ParseValue(valueType);

						if(name.GetType() != keyType)
						{
							name = Convert.ChangeType(name, keyType);
						}

						table[name] = val;
						break;
				}
			}
		}


		public object ParseClass(Type type) 
		{
			object instance = Activator.CreateInstance(type);

			// {
			while (true) {
				switch (NextToken) {
					case TOKEN.NONE:
						return null;
					case TOKEN.COMMA:
						continue;
					case TOKEN.CURLY_OPEN:
						json.Read();
						continue;
					case TOKEN.CURLY_CLOSE:
						return instance;
					default:
						// name
						string name = ParseString(typeof(string)) as string;
						if (name == null) {
							return null;
						}

						// :
						if (NextToken != TOKEN.COLON) {
							return null;
						}
						// ditch the colon
						json.Read();

						// value
						System.Reflection.FieldInfo field = type.GetField(name);
					if(field != null)
					{
						object val = ParseValue(field.FieldType);

                        if(val != null && ((val.GetType() == field.FieldType) || (typeof(object) == field.FieldType)))
						{
							field.SetValue(instance, val);
						}
					}
					else
					{
						ParseValue(typeof(object));
					}

						break;
				}
			}
		}


		public object ParseArray(Type type) 
		{
			Type valueType = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType(); // TODO create not generic array, need know array size? or every time increase ^2 and cut at finish
			if(valueType == null)
			{
				valueType = objectType;
			}
			Type genericListType = typeof(List<>).MakeGenericType(valueType);
			var array = Activator.CreateInstance(genericListType) as IList;

			json.Read();

			// [
			var parsing = true;
			while (parsing) {
				TOKEN nextToken = NextToken;

				switch (nextToken) {
					case TOKEN.NONE:
						return null;
					case TOKEN.COMMA:
						continue;
					case TOKEN.SQUARED_CLOSE:
						parsing = false;
						break;
					default:
						object value = ParseByToken(nextToken, valueType);
						array.Add(value);
						break;
				}
			}

			if(!type.IsGenericType)
			{
				var valueArray = Array.CreateInstance(valueType, array.Count);
				int count = array.Count;
				while(count-- > 0)
				{
					valueArray.SetValue(array[count], count);
				}

				return valueArray;
			}

			return array;
		}


		public object ParseValue(Type type) 
		{
			TOKEN nextToken = NextToken;

			if(Json.JsonConverters.ContainsKey(type))
			{
				return ParseDictionary(defaultDictionaryType);
			}

			return ParseByToken(nextToken, type);
		}


		public object ParseByToken(TOKEN token, Type type) 
		{
			object val;

			switch (token)
			{
				case TOKEN.STRING:
					val = ParseString(type);
					break;

				case TOKEN.NUMBER:
					val = ParseNumber(type);
					break;

				case TOKEN.CURLY_OPEN:
					val = ParseObject(type);
					break;

				case TOKEN.SQUARED_OPEN:
					val = ParseArray(type);
					break;

				case TOKEN.TRUE:
					val = true;
					break;

				case TOKEN.FALSE:
					val = false;
					break;

				case TOKEN.NULL:
					val = null;
					break;

				default:
					val = null;
					break;
			}

//						Debug.Log(type + " : " + token + " : " + val);

			return val;
		}


		public object ParseString(Type type)
		{
			StringBuilder s = new StringBuilder();
			char c;

			// ditch opening quote
			json.Read();

			bool parsing = true;
			while (parsing) {

				if (json.Peek() == -1) {
					parsing = false;
					break;
				}

				c = NextChar;
				switch (c) {
					case '"':
						parsing = false;
						break;
					case '\\':
						if (json.Peek() == -1) {
							parsing = false;
							break;
						}

						c = NextChar;
						switch (c) {
							case '"':
							case '\\':
							case '/':
								s.Append(c);
								break;
							case 'b':
								s.Append('\b');
								break;
							case 'f':
								s.Append('\f');
								break;
							case 'n':
								s.Append('\n');
								break;
							case 'r':
								s.Append('\r');
								break;
							case 't':
								s.Append('\t');
								break;
							case 'u':
								var hex = new char[4];

								for (int i=0; i< 4; i++) {
									hex[i] = NextChar;
								}

								s.Append((char) Convert.ToInt32(new string(hex), 16));
								break;
						}
						break;
					default:
						s.Append(c);
						break;
				}
			}

			string parsedString = s.ToString();

			if(type.IsEnum)
			{
				return Enum.Parse(type, parsedString);
			}
			else if(type == typeof(Guid))
			{
				return new Guid(parsedString);
			}

			return parsedString;
		}


		public object ParseNumber(Type type) {
			string number = NextWord;

//			Debug.Log(number);
            if (type.IsEnum)
            {
                int value;
                int.TryParse(number, out value);

                return Enum.ToObject(type, value);
            }

			if(type == typeof(int)){
				int value;
				int.TryParse(number, out value);
				return value;
			}

			if(type == typeof(long)){
				long value;
				long.TryParse(number, out value);
				return value;
			}

			if(type == typeof(float)){
				float value;
				float.TryParse(number, out value);
				return value;
			}

			if(type == typeof(double)){
				double value;
				double.TryParse(number, out value);
				return value;
			}

			if (number.IndexOf('.') == -1) 
			{
				int parsedInt;
				Int32.TryParse(number, out parsedInt);
				return parsedInt;
			}

			float parseFloat;
			float.TryParse(number, out parseFloat);
			return parseFloat;
		}


		public void EatWhitespace() {
			while (Char.IsWhiteSpace(PeekChar)) {
				json.Read();

				if (json.Peek() == -1) {
					break;
				}
			}
		}

		public char PeekChar {
			get {
				return Convert.ToChar(json.Peek());
			}
		}

		public char NextChar {
			get {
				return Convert.ToChar(json.Read());
			}
		}

		public string NextWord {
			get {
				StringBuilder word = new StringBuilder();

				while (!IsWordBreak(PeekChar)) {
					word.Append(NextChar);

					if (json.Peek() == -1) {
						break;
					}
				}

				return word.ToString();
			}
		}

		public TOKEN NextToken {
			get {
				EatWhitespace();

				if (json.Peek() == -1) {
					return TOKEN.NONE;
				}

				switch (PeekChar) {
					case '{':
						return TOKEN.CURLY_OPEN;
					case '}':
						json.Read();
						return TOKEN.CURLY_CLOSE;
					case '[':
						return TOKEN.SQUARED_OPEN;
					case ']':
						json.Read();
						return TOKEN.SQUARED_CLOSE;
					case ',':
						json.Read();
						return TOKEN.COMMA;
					case '"':
						return TOKEN.STRING;
					case ':':
						return TOKEN.COLON;
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '-':
						return TOKEN.NUMBER;
				}

				switch (NextWord) {
					case "false":
						return TOKEN.FALSE;
					case "true":
						return TOKEN.TRUE;
					case "null":
						return TOKEN.NULL;
				}

				return TOKEN.NONE;
			}
		}
	}

}