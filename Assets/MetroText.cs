using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MetroText : MonoBehaviour {
	// 对应的楼层的id
	private int[] humanGeneratingCounts;
	private int[] humanLeavingCounts;
	private Text text;

	private const int METRO_NUMS = 6;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		humanLeavingCounts = new int[METRO_NUMS];
		humanGeneratingCounts = new int[METRO_NUMS];
	}
//	const string TEMPLATE_STRING = 
//		"1: {}";

	private void updateText() {
		string text_content = "";
		for (int i = 1; i <= METRO_NUMS; i++) {
			text_content += $"{i}->coming: {humanGeneratingCounts[i - 1]}, Leaving: {humanLeavingCounts[i-1]}\n";
		}

		text.text = text_content;
	}
		

	private void updatingWithArray(int index, int new_nums, int[] n_array) {
		if (index >= METRO_NUMS || index < 0) {
			Debug.LogError ("Index " + index + " didn't exists.");
			return;
		} else {
			n_array [index] = new_nums;
			updateText ();
		}
	}

	public void UpdatingGenerating(int index, int new_nums) {
		updatingWithArray (index, new_nums, humanGeneratingCounts);
	}

	public void UpdatingLeaving(int index, int new_nums) {
		updatingWithArray (index, new_nums, humanLeavingCounts);

	}

}
