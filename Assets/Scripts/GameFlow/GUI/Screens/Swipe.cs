using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {

    private bool isTapped;

    private Vector2 startTouch;

    private Vector2 swipeDelta;

    public Vector2 SwipeDelta
    {
        get{ return swipeDelta;}
    }



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    void Reset()
    {
        startTouch = Vector2.zero;
        swipeDelta = Vector2.zero;

    }
}
