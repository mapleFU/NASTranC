using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class ExitReader: PureRectangleReader {
	// exit is just a line.
	public ExitReader(TextReader reader): base(reader, "Exit") {}

}
