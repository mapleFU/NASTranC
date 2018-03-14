using UnityEngine;
using System.Collections;

/*
 * 读取
 */ 
public class StairExitReader : BaseReader
{
	int[] args;
	Point[] line;

	const string LINE = "StairExit";
	public void parse() {

		for (int i = 0; i < 3; ++i) {
			args [i] = parse_line_to_int ();
		}
				
	}

	public void generate() {
		
	}

	public string get_class_name () {
		return LINE;
	}
}

