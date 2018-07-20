using UnityEngine;

public class QuadBez 
{
	public Vector3 st, en, ctrl;

	public QuadBez() 
	{
		st = en = ctrl = Vector3.zero;
	}

	public QuadBez(Vector3 st, Vector3 en, Vector3 ctrl) 
	{
		this.st = st;
		this.en = en;
		this.ctrl = ctrl;
	}
	
	public Vector3 Interp(float t) 
	{
		float d = 1f - t;
		return d * d * st + 2f * d * t * ctrl + t * t * en;
	}
	
	public Vector3 Velocity(float t) 
	{
		return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
	}

    public override string ToString()
    {
        return string.Format("[QuadBez]: + st: {0}   en: {1}    ctrl: {2}", st, en, ctrl);
    }
}