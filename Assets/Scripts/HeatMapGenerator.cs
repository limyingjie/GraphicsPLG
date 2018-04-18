using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeatMapGenerator
{

    public static HeatMap GenerateHeatMap(int width, int height, HeightMap heightMap, HeatMapSettings settings, Vector2 sampleCentre)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

        AnimationCurve heatCurve_threadsafe = new AnimationCurve(settings.heatCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heatCurve_threadsafe.Evaluate(values[i, j]) * settings.heatMultiplier;
				values [i, j] -= heightMap.values [i, j] * .15f;
				float heightPercentage = Mathf.Lerp(heightMap.minValue, heightMap.maxValue, heightMap.values [i, j]);
				if (heightPercentage < .2f) {
					values [i, j] = -2.5;
				}
                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }
		Debug.Log ("heat min = " + minValue);
		Debug.Log ("heat max = " + maxValue);
        return new HeatMap(values, minValue, maxValue);
    }

}

public struct HeatMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeatMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

