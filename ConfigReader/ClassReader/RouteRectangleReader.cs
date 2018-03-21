using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RouteRectangleReader : BaseReader {
	public RouteRectangleReader(TextReader reader): base(reader) {}
	const string BASIC_CONFIG = "routeRectangle";

	private bool is_positive;
	Point[] point_pairs;
	public override void generate () {

	}

	public override void parse() {

		int positive; // 是否是正数1
		// read if it is positive
		positive = int.Parse(reader.ReadLine());
		is_positive = (positive == 1);
		point_pairs = parse_point_pairs ();

	}

	public override string get_class_name () {
		return BASIC_CONFIG;
	}
}
