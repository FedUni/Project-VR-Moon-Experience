Shader "Hidden/TerrainEvo/Capture" {
Properties {
	/*[HideInInspector]*/ //_MainTex ("Texture", 2D) = "white" {}
	/*[HideInInspector]*/ u_Heightmap("u_Heightmap", 2D) = "black"  {}
	/*[HideInInspector]*/ u_NoiseTexture("Noise", 2D) = "white" {}
	/*[HideInInspector]*/ u_Controlmap("Control Map", 2D) = "white" {}

	/*[HideInInspector]*/ u_TextureSetsAlbedo("u_TextureSetsAlbedo", 2D) = "white" {}
	/*[HideInInspector]*/ u_TextureSetsSpecSmooth("u_TextureSetsSpecSmooth", 2D) = "white" {}
	/*[HideInInspector]*/ u_TextureSetsOcclusion("u_TextureSetsOcclusion", 2D) = "white" {}
	/*[HideInInspector]*/ u_TextureSetsNormal("u_TextureSetsNormal", 2D) = "white" {}
	/*[HideInInspector]*/ u_TextureSetsDisplacement("u_TextureSetsDisplacement", 2D) = "white" {}
}

SubShader {
	CGINCLUDE

#pragma target 5.0
#pragma only_renderers d3d11

#pragma vertex vertex
#pragma hull hull
#pragma domain domain
#pragma fragment fragment

#include "UnityCG.cginc"
#include "UnityPBSLighting.cginc"

#include "TETErrain_Inputs.cginc"
#include "TEBrushBase.cginc"
#include "TETErrain_Geometry.cginc"
#include "TETErrain_Shading.cginc"

	ENDCG

	Tags { "Special" = "TETerrain" }
	Pass {
		CGPROGRAM
			#pragma multi_compile TE_CAPTURE_MATERIAL_ID

			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
		ENDCG
    }

}
Fallback Off
}
