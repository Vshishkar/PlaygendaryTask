using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour {


    public tk2dUIItem swipeButton;

    public GameObject dot;

    private bool isTaped = false;
    private Vector2 startTouch;
    private Vector2 touchDelta;
    private Vector2 dotStartPosition;



    void Start () 
    {
        dot = GameObject.Find("DotPosition");
        CustomDebug.Log(dot.ToString() + " Dot start poSition " + dot.transform.position );
        dotStartPosition = dot.transform.position;
    }

    void OnEnable ()
    {
        swipeButton.OnClick += SwipeButton_OnClick;
    }


    void OnDisable()
    {
        swipeButton.OnClick -= SwipeButton_OnClick;
    }

    //TODO refactoring 
    void SwipeButton_OnClick ()
    {
       // CustomDebug.Log("Swippe button Works ");

        if (Input.GetMouseButtonDown(0))
        {
            isTaped = true;
                                
        }
        else if (Input.GetMouseButtonUp(0))
        {          
            Vector2 currentPosition = Input.mousePosition;
            CustomDebug.Log("Mouse Position is " + currentPosition.ToString()); 
            Vector2 dotCurrentPosition = dot.transform.position;
            dotCurrentPosition.x = currentPosition.x;
            dot.transform.position = dotCurrentPosition;
        }
        else
        {
            CustomDebug.Log("At else  :( ");
        }

    }

}
