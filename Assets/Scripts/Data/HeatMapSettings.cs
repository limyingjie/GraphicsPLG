using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeatMapSettings : UpdatableData {

	public NoiseSettings noiseSettings;

	public bool useFalloff;

	public float heatMultiplier;
	public AnimationCurve heatCurve;

	public float minHeight {
		get {
			return heatMultiplier * heatCurve.Evaluate (0);
		}
	}

	public float maxHeight {
		get {
			return heatMultiplier * heatCurve.Evaluate (1);
		}
	}

	#if UNITY_EDITOR

	protected override void OnValidate() {
		noiseSettings.ValidateValues ();
		base.OnValidate ();
	}
	#endif

}
