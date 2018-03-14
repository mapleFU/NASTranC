using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteRectangleReader : BaseReader {
	const string BASIC_CONFIG = "routeRectangle";

	protected bool is_positive;
	protected Point[] point_pairs;
	public void generate () {

	}

	public void parse() {
		checked_line (BASIC_CONFIG);
		int positive; // 是否是正数1
		// read if it is positive
		positive = int.Parse(reader.ReadLine());
		is_positive = (positive == 1);
		point_pairs = parse_point_pairs ();
		checked_line (BASIC_CONFIG);
	}
}
