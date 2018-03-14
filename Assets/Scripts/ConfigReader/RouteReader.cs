using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RouteReader : BaseReader {
	public RouteReader(TextReader reader): base(reader) {}

	const string BASIC_CONFIG = "Route";
	public char shape_order;
	// 未知的行
	const string UNKNOWN_LINE = "Routeshapeorder";
	public override void generate () {

	}

	public override void parse() {
		
		// routeRectangle
		// Routeshapeorder ??
		RouteRectangleReader rectangle_reader = new RouteRectangleReader(reader);
		rectangle_reader.parse ();
		rectangle_reader.generate ();

		// 获取状态，老实说我也不知道这是在搞什么
		checked_line (UNKNOWN_LINE);
		shape_order = reader.ReadLine ()[0];
		checked_line (UNKNOWN_LINE);

	}

	public override string get_class_name () {
		return BASIC_CONFIG;
	}
}
