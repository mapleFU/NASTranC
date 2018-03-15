using UnityEngine;
using System.Collections;
using System.IO;

/*
 * 读取
 */ 
public class StairExitReader : BaseReader
{
	public StairExitReader(TextReader reader): base(reader) {}
	int[] args;
	Point[] line;

	const string LINE = "StairExit";
	public override void parse() {

		line = parse_point_pairs ();
		for (int i = 0; i < 3; ++i) {
			args [i] = parse_line_to_int ();
		}
				
	}

	public override void generate() {
		
	}

	public override string get_class_name () {
		return LINE;
	}
}

