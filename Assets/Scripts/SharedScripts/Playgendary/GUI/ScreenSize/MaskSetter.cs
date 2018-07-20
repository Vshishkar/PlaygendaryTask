using UnityEngine;

[AddComponentMenu("Inventain/Size/MaskSetter")]
public class MaskSetter : SizeSetter {
	
	public SizeFactor maskFactorType = SizeFactor.MinFactor;
	public SizeFactor positionFactorType = SizeFactor.MinFactor;
	
	protected override void UpdateSize() {
		SizeHelper.RecalculateMask(GetComponent<tk2dUIMask>(), maskFactorType, roundFloatPreference);
		SizeHelper.RecalculatePosition(transform, positionFactorType, roundFloatPreference);
	}
}
