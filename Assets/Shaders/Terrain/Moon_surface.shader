Shader "Nature/Terrain/Moon_surface" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	
	[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
	_Mask1 ("Mask1 (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "black" {}
	[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "black" {}
	[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "black" {}
	[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	
	_Blur ("Blur", Range (0.01, 1)) = 0.02
	
	_ColorTex ("ColorMap (RGB)", 2D) = "black" {}

	_Normalmap ("Normalmap (RGB)", 2D) = "white" {}
	
	_HeightSplatAll ("Grass(R) Cliff(G) Stones(B) Snow(a)", 2D) = "black" {}
	_Parallax ("Height", Range (0.005, 0.08)) = 0.02

}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0

void vert (inout appdata_full v)
{
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;
}

sampler2D _Control;
sampler2D _Mask1;
sampler2D _Normalmap;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
half _Shininess;
float _Parallax;

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
	float3 worldPos;
	float _Parallax;
	float3 worldRefl;
	float3 viewDir;
	INTERNAL_DATA
};


		fixed4 _Color;
		half _Blur;
		sampler2D _ColorTex;
		sampler2D _HeightSplatAll;
		samplerCUBE _Cube;

void surf (Input IN, inout SurfaceOutput o) {
			half4 h = tex2D (_HeightSplatAll, IN.uv_Splat0).r;
			float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
			IN.uv_Splat0 += offset;
			
			half4 h2 = tex2D (_HeightSplatAll, IN.uv_Splat1).g;
			float2 offset2 = ParallaxOffset (h2, _Parallax, IN.viewDir);
			IN.uv_Splat1 += offset2;
			
			half4 h3 = tex2D (_HeightSplatAll, IN.uv_Splat3).a;
			float2 offset3 = ParallaxOffset (h3, _Parallax, IN.viewDir);
			IN.uv_Splat3 += offset3;

			half4 ColorTex = tex2D (_ColorTex, IN.uv_Control);
			half4 MaskTex = tex2D (_Mask1, IN.uv_Control);
			half4 Normalmap = tex2D (_Normalmap, IN.uv_Control);
			
			half4 Detail0 = tex2D (_Splat0, IN.uv_Splat0) * ColorTex;
			half4 Detail1 = tex2D (_Splat1, IN.uv_Splat1) * ColorTex;
			half4 Detail2 = tex2D (_Splat2, IN.uv_Splat2) * ColorTex;
			half4 Detail3 = tex2D (_Splat3, IN.uv_Splat3) * ColorTex;
			
			half4 HeightSplatTex1 = tex2D (_HeightSplatAll, IN.uv_Splat0 - offset).r;
			half4 HeightSplatTex2 = tex2D (_HeightSplatAll, IN.uv_Splat1 - offset2).g;
			half4 HeightSplatTex3 = tex2D (_HeightSplatAll, IN.uv_Splat2).b;
			half4 HeightSplatTex4 = tex2D (_HeightSplatAll, IN.uv_Splat3 - offset3).a;
			
			float a0 = MaskTex.r;
			float a1 = MaskTex.g;
			float a2 = MaskTex.b;
			float a3 = MaskTex.a;
			float a4 = MaskTex.g;
			
		    float ma = (max(max(max(HeightSplatTex1.rgb + a0, HeightSplatTex2.rgb + a1), HeightSplatTex3.rgb + a2), HeightSplatTex4.rgb + a3)) - _Blur;

		    float b0 = max(HeightSplatTex1.rgb + a0 - ma, 0);
		    float b1 = max(HeightSplatTex2.rgb + a1 - ma, 0);
		    float b2 = max(HeightSplatTex3.rgb + a2 - ma, 0);
		    float b3 = max(HeightSplatTex4.rgb + a3 - ma, 0)*6;
		    
		    float4 texture0 = Detail0;
		    float4 texture1 = Detail1;
		    float4 texture2 = Detail2;
		    float4 texture3 = Detail3;
		    fixed4 tex = (texture0 * b0 + texture1 * b1 + texture2 * b2 + texture3 * b3) / (b0 + b1 + b2 + b3);

		    tex = tex;

			texture0 = tex2D (_Normal0, IN.uv_Splat0);
			texture1 = tex2D (_Normal1, IN.uv_Splat1);
			texture2 = tex2D (_Normal2, IN.uv_Splat2);
			texture3 = tex2D (_Normal3, IN.uv_Splat3);
			float4 mixnormal = (texture0 * b0 + texture1 * b1 + texture2 * b2 + texture3 * b3) / (b0 + b1 + b2 + b3);
			
			o.Normal = 1.5*normalize(20 * UnpackNormal(mixnormal)+ UnpackNormal(Normalmap));
			//o.Normal = 1.5*normalize(20 * UnpackNormal(nrm) + UnpackNormal(tex2D(_MainBumpMap, IN.uv_Control)));
			//o.Normal = UnpackNormal(mixnormal);

			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
}
ENDCG  
}

Fallback "Nature/Terrain/Diffuse"
}
