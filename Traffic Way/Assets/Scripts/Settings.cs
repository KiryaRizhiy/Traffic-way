using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static string googlePlayId = "3577053";
    public static bool testMode = true;
    public static string savePath 
    { get { return Application.persistentDataPath + "/trafficWaySaves/"; } }
    public static string saveFile 
    { get { return savePath + "Save.svg"; } }
}
public enum PlacementType { video, rewardedVideo, banner }
