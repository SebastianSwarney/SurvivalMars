Shader "Custom/Terrain"
{
	Properties
	{
		_Color("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
		[HideInInspector]
		_TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" { }
	}
	SubShader
	{
		Pass
		{
			// indicate that our pass is the "base" pass in forward
			// rendering pipeline. It gets ambient and main directional
			// light data set up; light direction in _WorldSpaceLightPos0
			// and color in _LightColor0
			Tags {"LightMode" = "ForwardBase"}

			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // for _LightColor0

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;
				return o;
			}

			fixed4 _Color;
			sampler2D _TerrainHolesTexture;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 hole = tex2D(_TerrainHolesTexture, i.uv);
				if (hole.r < 1.0) {
					discard;
				}
				fixed4 col = fixed4(_Color.rgb * i.diff, 1.0);
				return col;
			}
			ENDCG
		}
	}
}