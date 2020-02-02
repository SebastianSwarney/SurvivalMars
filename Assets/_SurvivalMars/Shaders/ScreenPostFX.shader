Shader "Hidden/ScreenPostFX"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off
		//ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				const int chunkCount = 200;
				const float fChunkCount = float(chunkCount);
				const int2 chunk = int2(i.uv * chunkCount);
				const int n = chunk.x + chunk.y * chunkCount;
				float2 uv = i.uv;
				const int t = int(_Time.y * 20);
				if (t % 11 + t % 5 + t % 7 < 2)
				{
					const float2 relativeUv = i.uv - float2(chunk) / fChunkCount;
					const float2 seed = float2(sin(101.0 * n), sin(43.0 * n));
					const int2 shift = int2(sign(seed));
					uv = relativeUv + float2(chunk + shift) / fChunkCount;
				}
				uv.x *= 1.0 + .02 * float(frac(_Time.y / 10) < float(n) / fChunkCount / fChunkCount);
				fixed4 raw = tex2D(_MainTex, uv);
				float brightness = max(raw.x, max(raw.y, raw.z));
				fixed4 col = fixed4(0.1, 0.15 + 0.85 * brightness, 0.1, 1.0);
				return col;
			}
			ENDCG
		}
	}
}
