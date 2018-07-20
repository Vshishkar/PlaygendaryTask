using UnityEngine;
using System;


public static class TimeSpanExtension 
{
    public static string ToHHMMSS(this TimeSpan timeSpan)
    {
        string result = timeSpan.TotalSeconds > 0 ?
            string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) :
            "00:00:00";
        
        return result;
    }

	public static string ToHHMM(this TimeSpan timeSpan)
	{
		string result = timeSpan.TotalSeconds > 0 ?
			string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes) :
			"00:00";

		return result;
	}

	public static string ToMMSS(this TimeSpan timeSpan)
	{
		string result = timeSpan.TotalSeconds > 0 ?
			string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) :
			"00:00";

		return result;
	}
}