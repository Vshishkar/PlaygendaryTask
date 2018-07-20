#include "UnityCG.cginc"


// RGB --- TO OTHER COLOR SPACES

static const float Epsilon = 1e-10;
 
inline fixed3 RGB2HCV(fixed3 RGB)
{
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return fixed3(H, C, Q.x);
}

inline fixed3 RGB2HSV(fixed3 RGB)
{
    fixed3 HCV = RGB2HCV(RGB);
    float S = HCV.y / (HCV.z + Epsilon);
    return fixed3(HCV.x, S, HCV.z);
}

inline fixed3 RGB2HSL(fixed3 RGB)
{
    fixed3 HCV = RGB2HCV(RGB);
    float L = HCV.z - HCV.y * 0.5;
    float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
    return fixed3(HCV.x, S, L);
}





// OTHER COLOR SPACES -- TO RGB

inline fixed3 HUE2RGB(float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(fixed3(R,G,B));
}


inline fixed3 HSV2RGB(fixed3 HSV)
{
    fixed3 RGB = HUE2RGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

inline fixed3 HSL2RGB(fixed3 HSL)
{
    fixed3 RGB = HUE2RGB(HSL.x);
    float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
    return (RGB - 0.5) * C + HSL.z;
}




/**
// maybe more effective - need test

fixed3 RGB2HSV(fixed3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}


fixed3 HSV2RGB(fixed3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
**/