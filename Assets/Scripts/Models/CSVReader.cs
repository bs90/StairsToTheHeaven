using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader {
	public static List<Dictionary<string, object>> Read(string file)
	{
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load (file) as TextAsset;
		
		String[] lines = Regex.Split (data.text, Constants.LINE_SPLIT_RE);
		
		if (lines.Length <= 1) return list;
		
		String[] header = Regex.Split(lines[0], Constants.SPLIT_RE);
		for (int i = 1; i < lines.Length; i++) {
			
			String[] values = Regex.Split(lines[i], Constants.SPLIT_RE);
			if (values.Length == 0 ||values[0] == "") continue;
			
			var entry = new Dictionary<string, object>();
			for (var j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(Constants.TRIM_CHARS).TrimEnd(Constants.TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if (int.TryParse(value, out n)) {
					finalvalue = n;
				} 
				else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add (entry);
		}
		return list;
	}
}