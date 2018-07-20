#include "UnityCG.cginc"

// RGB --- TO OTHER COLOUR SPACES

/** 
    // RARE COLOR SPACES
    fixed3 RGB2HCY(fixed3 RGB)
    {
        // Corrected by David Schaeffer
        fixed3 HCV = RGB2HCV(RGB);
        float Y = dot(RGB, HCYwts);
        float Z = dot(HUE2RGB(HCV.x), HCYwts);
        if (Y < Z)
        {
            HCV.y *= Z / (Epsilon + Y);
        }
        else
        {
            HCV.y *= (1 - Z) / (Epsilon + 1 - Y);
        }
        return fixed3(HCV.x, HCV.y, Y);
    }

    static const float HCLgamma = 3;
    static const float HCLy0 = 100;
    static const float HCLmaxL = 0.530454533953517; // == exp(HCLgamma / HCLy0) - 0.5
    static const float PI = 3.1415926536;

    fixed3 RGB2HCL(fixed3 RGB)
    {
        fixed3 HCL;
        float H = 0;
        float U = min(RGB.r, min(RGB.g, RGB.b));
        float V = max(RGB.r, max(RGB.g, RGB.b));
        float Q = HCLgamma / HCLy0;
        HCL.y = V - U;
        if (HCL.y != 0)
        {
            H = atan2(RGB.g - RGB.b, RGB.r - RGB.g) / PI;
            Q *= U / V;
        }
        Q = exp(Q);
        HCL.x = frac(H / 2 - min(frac(H), frac(-H)) / 6);
        HCL.y *= Q;
        HCL.z = lerp(-U, V, Q) / (HCLmaxL * 2);
        return HCL;
    }
**/

// OTHER COLOUR SPACES -- TO RGB

/**
    // RARE COLOR SPACES

    // The weights of RGB contributions to luminance.
    // Should sum to unity.
    fixed3 HCYwts = fixed3(0.299, 0.587, 0.114);
     
    fixed3 HCY2RGB(fixed3 HCY)
    {
        fixed3 RGB = HUE2RGB(HCY.x);
        float Z = dot(RGB, HCYwts);
        if (HCY.z < Z)
        {
          HCY.y *= HCY.z / Z;
        }
        else if (Z < 1)
        {
          HCY.y *= (1 - HCY.z) / (1 - Z);
        }
        return (RGB - Z) * HCY.y + HCY.z;
    }

    fixed3 HCL2RGB(fixed3 HCL)
    {
        fixed3 RGB = 0;
        if (HCL.z != 0)
        {
            float H = HCL.x;
            float C = HCL.y;
            float L = HCL.z * HCLmaxL;
            float Q = exp((1 - C / (2 * L)) * (HCLgamma / HCLy0));
            float U = (2 * L - C) / (2 * Q - 1);
            float V = C / Q;
            float T = tan((H + min(frac(2 * H) / 4, frac(-2 * H) / 8)) * PI * 2);
            H *= 6;
            if (H <= 1)
            {
                RGB.r = 1;
                RGB.g = T / (1 + T);
            }
            else if (H <= 2)
            {
                RGB.r = (1 + T) / T;
                RGB.g = 1;
            }
            else if (H <= 3)
            {
                RGB.g = 1;
                RGB.b = 1 + T;
            }
            else if (H <= 4)
            {
                RGB.g = 1 / (1 + T);
                RGB.b = 1;
            }
            else if (H <= 5)
            {
                RGB.r = -1 / T;
                RGB.b = 1;
            }
            else
            {
                RGB.r = 1;
                RGB.b = -T;
            }
            RGB = RGB * V + U;
        }
        return RGB;
    }
**/
