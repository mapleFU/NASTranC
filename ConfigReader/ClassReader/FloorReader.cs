using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FloorReader : BaseReader {
	public FloorReader(TextReader reader): base(reader) {}

	const string BASIC_CONFIG = "Floor";

	public override void generate () {
		
	}

	public override void parse() {
		
	}

	public override string get_class_name () {
		return BASIC_CONFIG;
	}
}
