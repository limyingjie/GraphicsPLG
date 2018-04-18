Shader "Custom/UV2TEST" {

	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_MaskTex("Mask (RGB)", 2D) = "white" {}

	}

		SubShader{
		Tags{ "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha

		sampler2D _MainTex;
	sampler2D _MaskTex;


	struct Input {
		float2 uv_MainTex : TEXCOORD0;
		float2 uv2_MaskTex :   TEXCOORD1;
	};

	void surf(Input IN, inout SurfaceOutput surface) {
		half4 col = tex2D(_MainTex, IN.uv_MainTex.xy);
		half4 mask = tex2D(_MaskTex, IN.uv2_MaskTex.xy);
		//surface.Albedo = col.rgb;
		surface.Albedo = float3(IN.uv2_MaskTex.x, IN.uv2_MaskTex.x, IN.uv2_MaskTex.x);
		surface.Alpha = mask.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}