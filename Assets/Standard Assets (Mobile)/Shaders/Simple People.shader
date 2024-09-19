Shader "Custom/Simple People" {
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
		
		CGPROGRAM
		#pragma surface surf Lambert
		
		uniform sampler2D _MainTex;      // Main texture
        uniform sampler2D _HeadTex;      // Head texture
        uniform sampler2D _BodyTex;      // Body texture
        uniform sampler2D _LegsTex;      // Legs texture
        uniform sampler2D _BodyPartMask; // Body part mask
        uniform sampler2D _BloodMask;    // Blood mask
              
        half4 _BloodColor; // Blood color
        int _MaskIndex;    // Blood Mask index
        half4 _SkinColor;  // Skin color

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
		    // Create basic 
            half4 tex = tex2D (_MainTex, IN.uv_MainTex);
            half4 finalBlendTex = half4(0,0,0,0);
            half4 bodyPartMask = tex2D (_BodyPartMask, IN.uv_MainTex);
                			
            // Clothing textures
            half4 head = tex2D (_HeadTex, IN.uv_MainTex); // Head texture
            half4 body = tex2D (_BodyTex, IN.uv_MainTex); // Body texture
            half4 legs = tex2D (_LegsTex, IN.uv_MainTex); // Legs texture
                
            // Final blending clothing by body part
            half4 b = lerp (finalBlendTex, head, bodyPartMask.r);
            b = lerp (b, body, bodyPartMask.g);
            b = lerp (b, legs, bodyPartMask.b);
                
            // Apply to skin color
            half4 c = lerp (_SkinColor, tex, tex.a);
            c = lerp (c, b, b.a);
                
            // Create blood
            half4 blood = tex2D (_BloodMask, IN.uv_MainTex);
            half mask = blood.r;	
                
            // Blending blood by mask index		
            mask = lerp (mask, blood.g, step (0.9, _MaskIndex));
            mask = lerp (mask, blood.b, step (1.9, _MaskIndex));
                
            // Apply blood
            c = lerp (c, _BloodColor, mask);	
            // Apply to surface	       
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Custom/Simple People Vertex Lit"
}
