﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Localization
{
    public enum Language {Ru,En};
    private static String[] texts;
    private static String[] indexes;
    private const char Separator = ';';

    public static void LoadLocals(Language lang)
    {
        TextAsset _txt = Resources.Load<TextAsset>("TrafficWay/Other/Locals");
        String[] _full = _txt.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        Debug.Log("Locals file rows:" + Environment.NewLine + Functions.EnumerableAsString(_full));
        int rowNum = Array.FindAll<Char>(Regex.Replace(_full[0], lang.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == Separator).Length;
        Debug.Log("Lang row is: " + rowNum);
        texts = new String[_full.Length-1];
        indexes = new String[_full.Length-1];
        for (int i = 0; i < _full.Length - 1; i++)
        {
            indexes[i] = _full[i].Split(Separator)[0];
            texts[i] = Regex.Match(_full[i], @"(?<=^([^" + Separator + @"]+" + Separator + @"){" + rowNum + @"})[^" + Separator + @"]*").Value;
        }
        Debug.Log("Locals loaded. Indexes:" + Environment.NewLine + Functions.EnumerableAsString(indexes) + Environment.NewLine + "Texts:" +Environment.NewLine + Functions.EnumerableAsString(texts));
    }
    public static string GetLocal(string code)
    {
        return texts[Array.IndexOf(indexes, code)];
    }
}