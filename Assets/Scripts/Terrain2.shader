Shader "Custom/Terrain2" {
	Properties{
		testTexture("Texture", 2D) = "white"{}
		_Heat("Mask (RGB)", 2D) = "white" {}
		testScale("Scale", Float) = 1

	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		const static int maxLayerCount = 8;
	const static float epsilon = 1E-4;

	int layerCount;
	float3 baseColours[maxLayerCount];
	float baseStartHeights[maxLayerCount];
	float baseBlends[maxLayerCount];
	float baseColourStrength[maxLayerCount];
	float baseTextureScales[maxLayerCount];

	float minHeight;
	float maxHeight;

	sampler2D testTexture;
	float testScale;

	UNITY_DECLARE_TEX2DARRAY(baseTextures);

	struct Input {
		float3 worldPos;
		float3 worldNormal;
		float2 uv2_Heat   :   TEXCOORD1;
	};

	float inverseLerp(float a, float b, float value) {
		return saturate((value - a) / (b - a));
	}

	float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
		float3 scaledWorldPos = worldPos / scale;
		float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
		float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
		float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
		return xProjection + yProjection + zProjection;
	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
		float3 blendAxes = abs(IN.worldNormal);
		blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

		for (int i = 0; i < layerCount; i++) {
			float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

			float3 baseColour = baseColours[i] * baseColourStrength[i];
			float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

			o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
			if (i == 2) {
				drawStrength = inverseLerp(-baseBlends[5] / 2 - epsilon, baseBlends[5] / 2, 0.2 - IN.uv2_Heat.y);
				float3 textureColour = triplanar(IN.worldPos, baseTextureScales[5], blendAxes, 5);
				o.Albedo = o.Albedo * (1 - drawStrength) + textureColour * drawStrength;

				drawStrength = inverseLerp(-baseBlends[1] / 2 - epsilon, baseBlends[1] / 2, IN.uv2_Heat.y - 0.6);
				textureColour = triplanar(IN.worldPos, baseTextureScales[1], blendAxes, 1);
				o.Albedo = o.Albedo * (1 - drawStrength) + textureColour * drawStrength;
			}
		}

		//if (IN.uv2_Heat.x >0.6) {
		//	float3 textureColour = triplanar(IN.worldPos, baseTextureScales[1], blendAxes, 1);
		//	o.Albedo = textureColour;
		//}
		//if (IN.uv2_Heat.x <0.1) {
		//	float3 textureColour = triplanar(IN.worldPos, baseTextureScales[5], blendAxes, 5);
		//	o.Albedo = textureColour;
		//}

		//if (heightPercent >= baseStartHeight[2] && heightPercent <= baseStartHeight[3]) {
		//	float drawStrength = inverseLerp(-baseBlends[1] / 2 - epsilon, baseBlends[1] / 2, IN.uv2_Heat.x - 0.3);

		//	float3 baseColour = baseColours[1] * baseColourStrength[1];
		//	float3 textureColour = triplanar(IN.worldPos, baseTextureScales[1], blendAxes, 1) * (1 - baseColourStrength[1]);

		//	o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
		//}
		//o.Albedo = float3(IN.uv2_Heat.x, IN.uv2_Heat.x, IN.uv2_Heat.x);

	}


	ENDCG
	}
		FallBack "Diffuse"
}
