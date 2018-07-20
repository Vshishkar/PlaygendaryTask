using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CubicFunction : MonoBehaviour 
{
    #region Public Methods

    static public List<float> Solutions(float a, float b, float c, float d)
    {
        List<float> solutions = new List<float>();

        float aValue = b / a;
        float bValue = c / a;
        float cValue = d / a;

        float Q = (Mathf.Pow(aValue, 2f) - 3f * bValue) / 9f;
        float R = (2f * Mathf.Pow(aValue, 3f) - 9f * aValue * bValue + 27f * cValue) / 54f;

        float Q3 = Mathf.Pow(Q, 3f);
        float R2 = Mathf.Pow(R, 2f);

        if (R2 < Q3)
        {
            float t = Mathf.Acos(R / Mathf.Sqrt(Mathf.Pow(Q, 3f))) / 3f;

            solutions.Add(-2f * Mathf.Sqrt(Q) * Mathf.Cos(t) - aValue / 3f);
            solutions.Add(-2f * Mathf.Sqrt(Q) * Mathf.Cos(t + (2f * Mathf.PI / 3f)) - aValue / 3f);
            solutions.Add(-2f * Mathf.Sqrt(Q) * Mathf.Cos(t - (2f * Mathf.PI / 3f)) - aValue / 3f);
        }
        else
        {
            float coefA = -Mathf.Sign(R) * Mathf.Pow(Mathf.Abs(R) + Mathf.Sqrt(R2 - Q3), 1f / 3f);
            float coefB = 0f;

            if (coefA != 0)
            {
                coefB = Q / coefA;
            }

            solutions.Add((coefA + coefB) - aValue / 3f);

            if ( (coefA == coefB) && coefA > 0f)
            {
                solutions.Add(-coefA - aValue / 3f);
            }
        }

        return solutions;
    }


    #endregion
}
