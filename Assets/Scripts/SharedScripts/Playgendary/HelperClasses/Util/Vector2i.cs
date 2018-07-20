using UnityEngine;
using System.Collections;


[System.Serializable]
public struct Vector2i
{
	#region Fields

	public int x;
	public int y;

	#endregion


	#region Constructor

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	#endregion


    #region Public methods

    public Vector3 ConvertToV3()
    {
        return new Vector3((float)this.x, (float)this.y);
    }


    public static int Distance(Vector2i v1, Vector2i v2)
    {
        return (Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y));
    }        

    #endregion


	#region Constants

	public static Vector2i zero
	{
		get
		{
			return new Vector2i (0, 0);
		}
	}


	public static Vector2i one
	{
		get
		{
			return new Vector2i (1, 1);
		}
	}


	public static Vector2i left
	{
		get
		{
			return new Vector2i (-1, 0);
		}
	}


	public static Vector2i right
	{
		get
		{
			return new Vector2i (1, 0);
		}
	}


	public static Vector2i up
	{
		get
		{
			return new Vector2i (0, 1);
		}
	}


	public static Vector2i down
	{		get
		{
			return new Vector2i (0, -1);
		}
	}
		
	#endregion


	#region Operations

	public static bool operator == (Vector2i v1, Vector2i v2)
	{
		return ((v1.x == v2.x) && (v1.y == v2.y));
	}


	public static bool operator != (Vector2i v1, Vector2i v2)
	{
		return ((v1.x != v2.x) || (v1.y != v2.y));
	}


    public static Vector2i operator + (Vector2i v1, Vector2i v2)
    {
        return new Vector2i(v1.x + v2.x, v1.y + v2.y);
    }

	#endregion


    #region Object Stuff

    public override bool Equals (object other)
    {
        if (!(other is Vector2i))
        {
            return false;
        }

        Vector2i vector = (Vector2i)other;

        return this.x.Equals (vector.x) && this.y.Equals (vector.y) ;
    }


    public override int GetHashCode ()
    {
        return this.x ^ this.y;
    }


    public override string ToString()
    {
        return "" + x + "  " + y;
    }

    #endregion
}
