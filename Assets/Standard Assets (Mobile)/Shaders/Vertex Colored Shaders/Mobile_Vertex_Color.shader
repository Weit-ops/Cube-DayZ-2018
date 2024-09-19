// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mobile Diffuse Vertex Color" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass 
		{
			Tags{"LightMode" = "Vertex"}

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f_vct
			{
			    UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed3 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert(appdata v)
			{
				v2f_vct o;
#if UNITY_VERSION > 550
				o.vertex = UnityObjectToClipPos(v.vertex);
#else
				o.vertex = UnityObjectToClipPos(v.vertex);
#endif
				o.color = v.color.rgb * ShadeVertexLights(v.vertex, v.normal);
				o.texcoord = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f_vct i) : SV_Target
			{
			    half4 c;

			    c.rgb = tex2D(_MainTex, i.texcoord) * i.color;
			    c.a   = 1.0;
			    UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			
			ENDCG
		} 
	} 
	FallBack "VertexLit"
}
