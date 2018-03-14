using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorReader : BaseReader {
	const string BASIC_CONFIG = "Floor";

	public void generate () {
		
	}

	public void parse() {
		checked_line (BASIC_CONFIG);

		checked_line (BASIC_CONFIG);
	}
}
