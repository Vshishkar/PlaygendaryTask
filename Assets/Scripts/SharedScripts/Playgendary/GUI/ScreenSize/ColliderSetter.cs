using UnityEngine;

[AddComponentMenu("Inventain/Size/ColliderSetter")]
public class ColliderSetter : SizeSetter {
	
	public SizeFactor colliderFactorType = SizeFactor.MinFactor;
	public Fit colliderFitType = Fit.DontFit;
	public SizeFactor positionFactorType = SizeFactor.MinFactor;

	protected override void UpdateSize() {
		SizeHelper.RecalculateCollider(GetComponent<BoxCollider>(), colliderFactorType, colliderFitType, roundFloatPreference);
		SizeHelper.RecalculatePosition(transform, positionFactorType, roundFloatPreference);
	}
}
