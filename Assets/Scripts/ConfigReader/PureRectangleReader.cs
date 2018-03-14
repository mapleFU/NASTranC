using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * 有一个点对的读取对象
 */ 
public abstract class PureRectangleReader : BaseReader {
	string demo; 	// 给出的需要匹配的字符串
	protected Point[] points;

	public PureRectangleReader (TextReader reader, string filter_string): base(reader) {
		demo = filter_string;
	}
		
	public void parse() {
		points = parse_point_pairs ();
	}

	public string get_class_name () {
		return demo;
	}
}
