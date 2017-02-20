Shader "Unlit/AlphaCutoffAA" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color Multiplier", Color) = (1, 1, 1, 1) // color
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
		_Blend ("Blend Range", Range(0,.2)) = 0.1
	}
	SubShader {
		Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
	
		Lighting Off
	
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};
	
				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				fixed _Blend;
				float4 _Color;
	
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					if(col.a < _Cutoff) {
						col.a = 0;
					}
					else if (col.a == 1) {
						col.a = 1.0;
					}
					else if(col.a < _Cutoff + _Blend) {
						col.a = (col.a - _Cutoff + (_Blend/100)) / _Blend;
					}
					else {
						col.a = 1.0;
					}
					col.r *= _Color.r;
					col.g *= _Color.g;
					col.b *= _Color.b;
					col.a *= _Color.a;
					return col;
				}
			ENDCG
		}
	}
}