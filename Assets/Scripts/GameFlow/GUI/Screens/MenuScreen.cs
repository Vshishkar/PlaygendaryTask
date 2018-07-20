using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuScreen : MonoBehaviour
{
    #region Variables

    [SerializeField] tk2dUIItem bonusButton;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        bonusButton.OnClick += BonusButton_OnClick;
    }


    void OnDisable()
    {
        bonusButton.OnClick -= BonusButton_OnClick;
    }

    #endregion


    #region Events handlers

    void BonusButton_OnClick()
    {
        CustomDebug.Log("Click ! its me ");
    }

    #endregion
}
