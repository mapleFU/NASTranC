using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
	public Text timerText;
	private float secondsCount;
	private int minuteCount;
	private int hourCount;
	void Update(){
		UpdateTimerUI();
	}
	//call this on update
	public void UpdateTimerUI(){
		//set timer UI
		secondsCount += Time.deltaTime;
		timerText.text = hourCount +"h:"+ minuteCount +"m:"+(int)secondsCount + "s";

	}    
}

