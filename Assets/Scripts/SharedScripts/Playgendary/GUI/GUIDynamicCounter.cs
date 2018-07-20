using UnityEngine;
using System;

using System.Text;


public class GUIDynamicCounter : MonoBehaviour
{
	#region Variables  

	public event Action OnCounterValueChanged;
    public event Action OnCounterValueChangeEnded;

	[SerializeField] tk2dTextMesh counterLabel;
    [SerializeField] float filterValue = 0.1f;

    [SerializeField] bool isRoundCounter = false;
    [SerializeField] int roundValue = 5;

	StringBuilder sb = new StringBuilder();

	string postfixString = string.Empty;
	string prefixString = string.Empty;

    float currentValue;
    float finalValue;

	#endregion


	#region Properties

	public tk2dTextMesh CounterLabel
	{
		get
		{
			return counterLabel;
		}
	}


	string CurrentPrefixString
	{
		get { return prefixString; }

		set 
		{ 
            if (!string.IsNullOrEmpty(value))
            {
                if (prefixString != value)
                {
                    prefixString = value; 
                }
            }
		}
	}


	string CurrentPostfixString
	{
		get { return postfixString; }

		set 
		{
            if (!string.IsNullOrEmpty(value))
            {
                if (postfixString != value)
                {
                    postfixString = value; 
                }
            }
		}
	}
		

	string CurrentValueString
	{
		get
		{
			if (sb.Length > 0)
			{
				sb.Remove(0, sb.Length);
			}

            sb.Append(CurrentPrefixString);
			sb.Append(currentValue.ToString("F0"));
            sb.Append(CurrentPostfixString);

			return sb.ToString();
		}
	}

	#endregion

  
	#region Unity Lifecycle   

    void Update()
    {
        if (Mathf.Abs(finalValue - currentValue) > float.Epsilon)  
        {
            int oldValue = Mathf.FloorToInt(currentValue);

			currentValue += (filterValue * Mathf.Abs(finalValue - oldValue));
			currentValue = Mathf.Min(currentValue, finalValue);

            if (oldValue != Mathf.FloorToInt(currentValue))
            {
                counterLabel.text = CurrentValueString;

                if (OnCounterValueChanged != null)
                {
                    OnCounterValueChanged();
                }
            }
            else
            {
                if (OnCounterValueChangeEnded != null)
                {
                    OnCounterValueChangeEnded();
                }
                
            }
        }
    }

	#endregion

 
	#region Public methods   

	public void SetValue(float v, bool immediately = false, string prefix = default(string), string postfix = default(string))
    {
        if (isRoundCounter)
        {
            float remainOfDevision = v % roundValue; 

            if (remainOfDevision < 1.0f)
            {
                
            }
            else
            {
                v += roundValue - remainOfDevision;
            }
        }

        CurrentPostfixString = postfix;
        CurrentPrefixString = prefix;

		if (immediately)
        {
            currentValue = finalValue = v;
			counterLabel.text = CurrentValueString;
        }
        else
        {
            finalValue = v;
        }
    }



	#endregion

}