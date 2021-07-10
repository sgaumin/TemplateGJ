/*
 * Internationalization
 *
 * Author: Daniel Erdmann
 *
 * 1. Add this File to you Project
 *
 * 2. Add the language files to the folder Assets/Resources/I18n. (Filesnames: en.txt, es.txt, pt.txt, de.txt, and so on)
 *    Format: en.txt:           es.txt:
 *           =============== =================
 *           |hello=Hello  | |hello=Hola     |
 *           |world=World  | |world=Mundo    |
 *           |...          | |...            |
 *           =============== =================
 *
 * 3. Use it!
 *    Debug.Log(I18n.Fields["hello"] + " " + I18n.Fields["world"]); //"Hello World" or "Hola Mundo"
 *
 * Use \n for new lines. Fallback language is "en"
 */

using System;
using System.Collections.Generic;
using UnityEngine;

internal class I18n
{
	/// <summary>
	/// Text Fields
	/// Useage: Fields[key]
	/// Example: I18n.Fields["world"]
	/// </summary>
	public static Dictionary<string, string> Fields { get; private set; }

	/// <summary>
	/// Init on first use
	/// </summary>
	static I18n()
	{
		LoadLanguageFromGameData();
	}

	public static void LoadLanguageFromGameData(Language forceLanguage = Language.None)
	{
		if (Fields == null)
			Fields = new Dictionary<string, string>();

		Fields.Clear();
		string lang = forceLanguage == Language.None ? GameData.CurrentLanguage.ToString() : forceLanguage.ToString();
		var textAsset = Resources.Load(@"I18n/" + lang); //no .txt needed
		string allTexts = "";
		if (textAsset == null)
			textAsset = Resources.Load(@"I18n/fr") as TextAsset; //no .txt needed
		if (textAsset == null)
			Debug.LogError("File not found for I18n: Assets/Resources/I18n/" + lang + ".txt");
		allTexts = (textAsset as TextAsset).text;
		string[] lines = allTexts.Split(new string[] { "\r\n", "\n" },
			StringSplitOptions.None);
		string key, value;
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#"))
			{
				key = lines[i].Substring(0, lines[i].IndexOf("="));
				value = lines[i].Substring(lines[i].IndexOf("=") + 1,
						lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
				Fields.Add(key, value);
			}
		}
	}
}