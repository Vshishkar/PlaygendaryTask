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


namespace MiniJSON {
	// Example usage:
	//
	//  using UnityEngine;
	//  using System.Collections;
	//  using System.Collections.Generic;
	//  using MiniJSON;
	//
	//  public class MiniJSONTest : MonoBehaviour {
	//      void Start () {
	//          var jsonString = "{ \"array\": [1.44,2,3], " +
	//                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
	//                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
	//                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
	//                          "\"int\": 65536, " +
	//                          "\"float\": 3.1415926, " +
	//                          "\"bool\": true, " +
	//                          "\"null\": null }";
	//
	//          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
	//
	//          CustomDebug.Log("deserialized: " + dict.GetType());
	//          CustomDebug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
	//          CustomDebug.Log("dict['string']: " + (string) dict["string"]);
	//          CustomDebug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
	//          CustomDebug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
	//          CustomDebug.Log("dict['unicode']: " + (string) dict["unicode"]);
	//
	//          var str = Json.Serialize(dict);
	//
	//          CustomDebug.Log("serialized: " + str);
	//      }
	//  }

	/// <summary>
	/// This class encodes and decodes JSON strings.
	/// Spec. details, see http://www.json.org/
	///
	/// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
	/// All numbers are parsed to doubles.
	/// </summary>
	public static class Json 
	{
		public static Dictionary<Type, JSONCustomConverter> JsonConverters = new Dictionary<Type, JSONCustomConverter>()
		{
			{typeof(DateTime), new JSONDateTimeConverter()},
			{typeof(AnimationCurve), new JSONAnimationCurveConverter()},
			{typeof(Keyframe), new JSONKeyFrameConverter()}
		};


		public static Dictionary<string, JSONCustomConverter> JsonConvertersString = new Dictionary<string, JSONCustomConverter>()
		{
			{"DateTime", new JSONDateTimeConverter()},
			{"AnimationCurve", new JSONAnimationCurveConverter()},
			{"Keyframe", new JSONKeyFrameConverter()}
		};



		public static T Deserialize<T>(string json) 
		{
			return (T)Deserialize(json, typeof(T));
		}


		public static object Deserialize(string json, Type type) 
		{
			if (json == null) 
			{
				return null;
			}

			return JSONParser.Parse(json, type);
		}


		public static string Serialize(object obj) 
		{
			return JSONSerializer.Serialize(obj);
		}


		public static void SerializeTest<T>(T obj)
		{
			string str = MiniJSON.Json.Serialize(obj);
			CustomDebug.Log(str);
			T t = MiniJSON.Json.Deserialize<T>(str);
			CustomDebug.Log(t);
			CustomDebug.Log(t.GetType());

			string str2 = MiniJSON.Json.Serialize(t);
			CustomDebug.Log(str2);
		}
	}
}