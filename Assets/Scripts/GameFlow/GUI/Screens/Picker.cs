using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picker : MonoBehaviour {

    #region Variables

    GameObject picker;

    #endregion

    #region UnityLifeCycle

	void Start () {
      
        picker = GetComponent<GameObject>(); 
        TweenColor.SetColor(picker, Color.red, 1f);

    }
	    
	// Update is called once per frame
	void Update () {
		
	}

    #endregion
}
