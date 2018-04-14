using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TreeHeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float treeHeightMultiplier;
    public AnimationCurve treeHeightCurve;

    public float minHeight
    {
        get
        {
            return treeHeightMultiplier * treeHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return treeHeightMultiplier * treeHeightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

	protected override void OnValidate() {
		noiseSettings.ValidateValues ();
		base.OnValidate ();
	}
#endif

}
