// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Simple People Vertex Lit" {
	Properties {
		_SkinColor ("Skin color", Color) = (0.96, 0.725, 0.62, 1) // Skin color
		_MainTex ("Main texture", 2D) = "black" {}                // Main texture
    	_HeadTex ("Head texture", 2D) = "black" {}                // Head texture
		_BodyTex ("Body texture", 2D) = "black" {}                // Body texture
		_LegsTex ("Legs texture", 2D) = "black" {}                // Legs texture
		_BodyPartMask ("Body part mask", 2D)= "black" {}          // Body part mask
		_BloodMask ("Blood mask", 2D)= "black" {}                 // Blood mask
		_BloodColor ("Blood color", Color) = (1, 0, 0, 1)         // Blood color
	}
	
	SubShader {
	    Tags { "RenderType"="Opaque" }
		LOD 200
		Cull back
		
	    Pass
	    {
	        Tags{"LightMode" = "Vertex"}
	        
	        CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;      // Main texture
            uniform sampler2D _HeadTex;      // Head texture
            uniform sampler2D _BodyTex;      // Body texture
            uniform sampler2D _LegsTex;      // Legs texture
            uniform sampler2D _BodyPartMask; // Body part mask
            uniform sampler2D _BloodMask;    // Blood mask
              
            half4 _BloodColor; // Blood color
            int _MaskIndex;    // Blood Mask index
            half4 _SkinColor;  // Skin color
            
            struct appdata 
			{
				float4 vertex : POSITION;
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
				o.color = ShadeVertexLights(v.vertex, v.normal) * 0.5f + UNITY_LIGHTMODEL_AMBIENT;
				o.texcoord = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
            
            float4 frag(v2f_vct i) : COLOR 
            {	
                // Create basic 
                half4 tex = tex2D (_MainTex, i.texcoord);
                half4 finalBlendTex = half4(0,0,0,0);
                half4 bodyPartMask = tex2D (_BodyPartMask, i.texcoord);
                			
                // Clothing textures
                half4 head = tex2D (_HeadTex, i.texcoord); // Head texture
                half4 body = tex2D (_BodyTex, i.texcoord); // Body texture
                half4 legs = tex2D (_LegsTex, i.texcoord); // Legs texture
                
                // Final blending clothing by body part
                half4 b = lerp (finalBlendTex, head, bodyPartMask.r);
                b = lerp (b, body, bodyPartMask.g);
                b = lerp (b, legs, bodyPartMask.b);
                
                // Apply to skin color
                half4 c = lerp (_SkinColor, tex, tex.a);
                c = lerp (c, b, b.a);
                
                // Create blood
                half4 blood = tex2D (_BloodMask, i.texcoord);
                half mask = blood.r;	
                
                // Blending blood by mask index		
                mask = lerp (mask, blood.g, step (0.9, _MaskIndex));
                mask = lerp (mask, blood.b, step (1.9, _MaskIndex));
                
                // Apply blood
                c  = lerp (c, _BloodColor, mask);	
                c *= half4(i.color.rgb, 1.0);
                UNITY_APPLY_FOG(i.fogCoord, c);     
                return c;
        	}
        	
        	ENDCG
	    }
	} 
	FallBack "VertexLit"
}