using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteReader : BaseReader {
	const string BASIC_CONFIG = "Route";

	public void generate () {

	}

	public void parse() {
		checked_line (BASIC_CONFIG);
		// routeRectangle
		// Routeshapeorder ??
		RouteRectangleReader rectangle_reader = new RouteRectangleReader(reader);

		checked_line (BASIC_CONFIG);
	}
}
