#include "ColorSpaces.cginc"

#define PHOTOSHOP_BLEND_HUE(BOTTOM, TOP, RESULT)            PHOTOSHOP_BLEND(BOTTOM, TOP, RESULT, hslTop.x, hslBottom.y, hslBottom.z)
#define PHOTOSHOP_BLEND_SATURATION(BOTTOM, TOP, RESULT)     PHOTOSHOP_BLEND(BOTTOM, TOP, RESULT, hslBottom.x, hslTop.y, hslBottom.z)
#define PHOTOSHOP_BLEND_COLOR(BOTTOM, TOP, RESULT)          PHOTOSHOP_BLEND(BOTTOM, TOP, RESULT, hslTop.x, hslTop.y, hslBottom.z)
#define PHOTOSHOP_BLEND_LUMINOSITY(BOTTOM, TOP, RESULT)     PHOTOSHOP_BLEND(BOTTOM, TOP, RESULT, hslBottom.x, hslBottom.y, hslTop.z)


#define PHOTOSHOP_BLEND(BOTTOM, TOP, RESULT, par1, par2, par3)  { \
    fixed3 hslTop = RGB2HSL(TOP.xyz); \
    fixed3 hslBottom = RGB2HSL(BOTTOM.xyz); \
    RESULT.xyz = HSL2RGB(fixed3(par1, par2, par3)); \
    \
}



// Example
// fixed4 colorBottom;
// fixed4 colorTop;
// fixed4 result;
// PHOTOSHOP_BLEND_SATURATION(colorBottom, colorTop, result);