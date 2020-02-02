Shader "Custom/Crystal"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
		SubShader
	{
		Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;

			struct appdata
			{
				float4 norm : NORMAL;
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 norm : NORMAL;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.norm = UnityObjectToClipPos(v.norm);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float t = sin(i.vertex.y / 100.0 + _Time.y) + 1.5;
				float shine = 1.0 + .5 * dot(normalize(i.vertex), normalize(i.norm));
				fixed3 col = _Color * t * shine;
				return fixed4(col, 1.0);
			}
			ENDCG
		}
	}
}
