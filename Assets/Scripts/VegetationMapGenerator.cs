using System;
using UnityEngine;

public static class VegetationMapGenerator
{
	public static TreeMap generateVegetationMap (int width, int height, HeightMap heightMap, HeatMap heatMap, TreeHeightMapSettings settings, Vector2 sampleCentre){
		float[,] treeHeightMap = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
		System.Random rng = new System.Random ();

		float[,] grassMap = new float[width, height];
		float[,] pTree = new float[width, height];

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				float pHeight = (Mathf.Cos (2 * Mathf.PI * (heightMap.values [x, y] - .5f)) + 1) / 2;
				float pHeat = (Mathf.Cos (2 * Mathf.PI * (heatMap.values [x, y] - .5f)) + 1) / 2;
				float p = pHeight * pHeat;
				bool hasTree = rng.Next (0, 100) > (100 - p * 100);
				float treeHeight = hasTree ? treeHeightMap [x, y] : 0;
				treeHeightMap [x, y] = treeHeight;
                if (treeHeight > maxValue)
                {
                    maxValue = treeHeight;
                }
                if (treeHeight < minValue)
                {
                    minValue = treeHeight;
                }
            }
		}

        return new TreeMap(treeHeightMap, minValue, maxValue);

    }


}


public struct TreeMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public TreeMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

//public struct VegetationMap {
//	public VegetationMap (HeightMap heightMap, TextureData textureData)
//	{

//	}
//}

//public struct Vegetation {

//	public readonly Vector2 position;
//	public readonly float size;

//	public Vegetation (Vector2 position, float size, VegetationType type) {
//		this.position = position;
//		this.size = size;
//		this.type = type;
//	}
//}

//public struct HeatMap {
//	// 1 = 40 celsius; 0 = -20 celsius
//	public readonly float[,] values;
//	public readonly float maxValue;
//	public readonly float minValue;
//}