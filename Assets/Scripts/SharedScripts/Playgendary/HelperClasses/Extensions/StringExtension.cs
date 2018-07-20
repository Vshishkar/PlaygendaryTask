using UnityEngine;
using System.Text;

public static class StringExtension 
{
    public static string GetShortNumberForm(this string s)
    {
        StringBuilder formattedText = new StringBuilder(s);
        
        int currentNumberCount = 0;
        for (int i = formattedText.Length - 1; i >= 0; i--)
        {
            bool digitCharacter = false;
            var curChar = formattedText[i];
            if (curChar >= '0' && curChar <= '9')
            {
                digitCharacter = true;
                currentNumberCount += 1;
            }
            if ((!digitCharacter) || ((digitCharacter) && (i == 0)))
            {
                if (currentNumberCount > 3)
                {
                    int numberStartIdx = (digitCharacter) ? i : (i + 1);

                    int lettersToCut = currentNumberCount > 6 ? 6 : 3;

                    string endLetter = lettersToCut == 6 ? "M" : "k";

                    int numberFinishIdx = numberStartIdx + currentNumberCount;
                    int pointPlace = numberFinishIdx - lettersToCut;
                    int cutStartIdx = pointPlace;
                    if (formattedText[cutStartIdx] != '0' && ((currentNumberCount - lettersToCut) <= 2))
                    {
                        formattedText.Insert(pointPlace, ".");
                        cutStartIdx += 2;
                        lettersToCut -= 1;
                    }
                    formattedText.Remove(cutStartIdx, lettersToCut);
                    formattedText.Insert(cutStartIdx, endLetter);
                }
                currentNumberCount = 0;
            }
        }

        return formattedText.ToString();
    }
	

    public static char ASCII_NL = '\n';
    public static char ASCII_WS = ' ';

    /// <summary>
    /// Remove first 32 non-printing characters, except new line, from string
    /// </summary>
    public static string RemoveCharsControl(this string s)
    {        
        if (s != null)
        {
            var len = s.Length;
            var src = s.ToCharArray();
            int dstIdx = 0;
            for (int i = 0; i < len; i++) {
                var ch = src[i];
                if ((ch < ASCII_WS) && (ch != ASCII_NL))
                {
                    continue;
                }
                src[dstIdx++] = ch;
            }
            s = new string(src, 0, dstIdx);
        }
        return s;
    }

    /// <summary>
    /// Remove new line character, from string
    /// </summary>
    public static string RemoveCharsNewline(this string s)
    {
        return s.RemoveChars(ASCII_NL);
    }

    /// <summary>
    /// Remove white space character, from string
    /// </summary>
    public static string RemoveCharsWhiteSpace(this string s)
    {
        return s.RemoveChars(ASCII_WS);
    }

    /// <summary>
    /// Remove selected character, from string
    /// </summary>
    public static string RemoveChars(this string s, char removeChar) 
    {
        if (s != null)
        {
            var len = s.Length;
            var src = s.ToCharArray();
            int dstIdx = 0;
            for (int i = 0; i < len; i++) {
                var ch = src[i];
                if (ch != removeChar)
                {
                    src[dstIdx++] = ch;
                    continue;
                }
            }
            s = new string(src, 0, dstIdx);
        }
        return s;
    }
}
