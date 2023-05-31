// Define a new shader named "OutlinedShader" in the "Custom" category
Shader "Custom/OutlinedShader" 
{
	//Properties that can be adjusted in the material inspector
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1) // Outline color with default white
		_MainTex("Albedo (RGB)", 2D) = "white" {} // Main texture with default white texture
		_OutlineWidth("Outline Width", Range(0, 5)) = 1 // Outline width with a slider range from 0 to 5, default 1
	}

	// Define the main subshader
	SubShader
	{
		// Set rendering queue and type to transparent for proper rendering order
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100 // Set the Level of Detail (LOD) to 100

		// Define the main CGPROGRAM block
		CGPROGRAM
			#pragma surface surf Lambert // Use Lambertian lighting model for the main object
			sampler2D _MainTex; // Declare the main texture

			// Define the input structure for the surface shader
			struct Input 
			{
				float2 uv_MainTex; // Texture coordinates for the main texture
			};

			fixed4 _Color; // Declare the outline color

			// Define the surface shader function
			void surf(Input IN, inout SurfaceOutput o) 
			{
				// Sample the main texture using the input texture coordinates
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb; // Assign the color to the output albedo
				o.Alpha = c.a; // Assign the alpha to the output alpha
			}

		ENDCG // End of the main CGPROGRAM block

		// Define the outline pass
		Pass 
		{
			Name "OUTLINE" // Name the pass "OUTLINE"
			// Set rendering queue and type to transparent for proper rendering order
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			Cull Front // Cull front faces of the mesh
			ZWrite Off // Disable writing to the depth buffer
			ZTest Always // Always pass the depth test
			ColorMask RGB // Only write the RGB channels to the color buffer

			// Define the outline CGPROGRAM block
			CGPROGRAM
			#pragma vertex vert // Use custom vertex shader function
			#pragma fragment frag // Use custom fragment shader function
			#include "UnityCG.cginc" // Include standard Unity shader library
			
			// Define the input structure for the vertex shader
			struct appdata 
			{
				float4 vertex : POSITION; // Vertex position
				float2 uv : TEXCOORD0; // Texture coordinates
			};
			
			// Define the output structure for the vertex shader
			struct v2f 
			{
				float2 uv : TEXCOORD0; // Texture coordinates
				float4 vertex : SV_POSITION; // Clip-space position
			};
	
			sampler2D _MainTex; // Declare the main texture
			float _OutlineWidth; // Declare the outline width
			fixed4 _Color; // Declare the outline color here, within the outline CGPROGRAM block

			// Define the vertex shader function
			v2f vert(appdata v) 
			{
				v2f o; // Create the output structure
				o.vertex = UnityObjectToClipPos(v.vertex); // Transform vertex position to clip space
				float3 normal = UnityObjectToWorldNormal(v.vertex.xyz); // Calculate world-space normal
				float2 offset = float2(normal.x, normal.y) * _OutlineWidth; // Calculate the outline offset using the normal and outline width
				o.vertex.xy += offset; // Add the offset to the vertex x and y coordinates
				o.uv = v.uv; // Pass the texture coordinates to the output structure
				return o; // Return the output structure
			}

			// Define the fragment shader function
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv); // Sample the main texture using the input texture coordinates
				// If the alpha of the sampled color is zero, return the outline color; otherwise, return a transparent color
				return col.a == 0 ? _Color : fixed4(0, 0, 0, 0);
			}
			ENDCG // End of the outline CGPROGRAM block
		}
	}
	FallBack "Diffuse" // Use the default Diffuse shader as a fallback
}