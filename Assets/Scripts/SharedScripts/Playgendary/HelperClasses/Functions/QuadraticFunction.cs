using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class QuadraticFunction : MonoBehaviour 
{
    #region Public Methods

    static public List<float> Solutions(float a, float b, float c, float d, float e)
    {
        List<float> solutions = new List<float>();

        float discriminant = Mathf.Pow(b, 3f) / 8f - b * c * 0.5f + d;

        if (discriminant != 0f)
        {  

            float cubicA = 1f;
            float cubicB = -c;
            float cubicC = (b * d - 4f * e);
            float cubicD = -(e * (Mathf.Pow(b, 2f) - 4f * c) + Mathf.Pow(d, 2f));

            List<float> cubicSolutions = CubicFunction.Solutions(cubicA, cubicB, cubicC, cubicD);

            solutions.Add(0.5f * (-0.5f * b + Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]) + Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.5f - c - cubicSolutions[0] - 2f * discriminant / Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]))));
            solutions.Add(0.5f * (-0.5f * b + Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]) - Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.5f - c - cubicSolutions[0] - 2f * discriminant / Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]))));
            solutions.Add(0.5f * (-0.5f * b - Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]) + Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.5f - c - cubicSolutions[0] + 2f * discriminant / Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]))));
            solutions.Add(0.5f * (-0.5f * b - Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]) - Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.5f - c - cubicSolutions[0] + 2f * discriminant / Mathf.Sqrt(Mathf.Pow(b, 2f) * 0.25f - c + cubicSolutions[0]))));
        }
        else
        {
            float x1 = (-c + Mathf.Sqrt(Mathf.Pow(c, 2f) - 4f * e)) * 0.5f;
            float x2 = (-c - Mathf.Sqrt(Mathf.Pow(c, 2f) - 4f * e)) * 0.5f;

            solutions.Add(Mathf.Sqrt(x1));
            solutions.Add(-Mathf.Sqrt(x1));
            solutions.Add(Mathf.Sqrt(x2));
            solutions.Add(-Mathf.Sqrt(x2));
        }

        return solutions;
    }

    #endregion
}
