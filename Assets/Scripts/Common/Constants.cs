using System.Collections;
using System.Collections.Generic;

public class Constants{
	// Levels
	public const float PLAYTIME = 60;
	public static readonly Dictionary<int, string[]> LEVELS = new Dictionary<int, string[]> {
		{ 0, new string[]{"start"} },
		{ 1, new string[]{"1"}},
		{ 2, new string[]{"2"}},
		{ 3, new string[]{"end"} },
	};

	// Because I don't like numbers
	public const int MIN_ONE = -1;
	public const int ZERO = 0;
	public const int ONE = 1;
	public const int TWO = 2;
	public const int TEN = 10;

	// CSV Related Constants
	public const string EXAMPLE_CSV = "CSV/CSVExample10";
	public const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	public const string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	public static readonly char[] TRIM_CHARS = { '\"' };
	
	// Messages Related Constants
	public const int MES_DISPLAY_COUNTER = 20;

	public const string MES_ID = "ID";
	public const string MES_TEXT_BOX = "MessageBox";
	public const string MES_NAME_BOX = "NameBox";
	public const string MES_SCENE = "Scene";
	public const string MES_CHARACTER = "Character";
	public const string MES_SKIPABLE = "Skipable";
	public const string MES_VOICED = "Voiced";
	public const string MES_TEXT = "Text";

	// Message files
	public const string MF_A01 = "CSV/CSVExample10";

	// String constants
	public const string EMPTY = "";
	public const string SPACE = " ";
	public const string UNDERSCORE = "_";
	public const string DOT = ".";
	public const string RESO_TIME = " x "; 	

	// Windows registry
	public const string WIN_REG = @"Software\";
	public const string WIN_SLASH = @"\";

	// MAC Registry
	public const string MAC_REG = "/Library/Preferences/unity.";
	public const string PLIST = "plist";
	public const string PLIST_FILE = ".plist";
	public const string DICT = "dict";
	public const string PLUTIL = "plutil";
	public const string CONVERT_XML = "-convert xml1 \"";
	public const string CONVERT_BIN = " -convert binary1 \"";
	public const string MAC_SLASH = "\"";

	// Application
	public const string LEVELS_PASSED = "LevelsPassed";
	public const string CONTROLLER = "Controller";
	public const float FOLLOW_CAMERA = 2.8f; 

	// Math
	public const float FLOATING_FORCE = 300f; //Newton

	// UI Config
	public const int DIS_BETWEEN_ROWS = 70;

	// Variables
	public const string STRING = "string";
	public const string INTEGER = "integer";
	public const string REAL = "real";
	public const string FLOAT = "float";
}
