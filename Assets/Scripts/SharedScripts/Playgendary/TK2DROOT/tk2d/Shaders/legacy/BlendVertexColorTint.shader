// unlit, vertex colour, alpha blended
// cull off

Shader "tk2d/BlendVertexColorTint" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_TintFactor ( "TintFactor", Range (0,1.0)) = 0
        _TintColor ( "TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float _TintFactor;
            uniform float4 _TintColor;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);

				float totalBrightness = (col.r + col.g + col.b) / 3.0;             

				col = float4(col.r * (1.0 - _TintFactor) + totalBrightness * _TintFactor * _TintColor.r,
                             col.g * (1.0 - _TintFactor) + totalBrightness * _TintFactor * _TintColor.g,
                             col.b * (1.0 - _TintFactor) + totalBrightness * _TintFactor * _TintColor.b,
                             col.a);

				return col;
			}
			
			ENDCG
		} 
	}
}
