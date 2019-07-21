using System.Collections.Generic;

public delegate void Invoke(string targetDirectory, bool? readOnly, bool? writeHeader, List<string> exclude);