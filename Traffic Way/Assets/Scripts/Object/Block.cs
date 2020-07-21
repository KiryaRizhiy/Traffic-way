using System.Collections;
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int length;
    public EnvironmentType environmentType;
    public EnvironmentClassification classification;
    private static GameObject[] Environments;
    private GameObject[] currentEnvironments;
    //{
    //    get
    //    {
    //        if (Environments == null || Environments.Length == 0)
    //        {
    //            Debug.LogError("No block environments loaded");
    //            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error,"No block environments loaded");
    //            return null;
    //        }
    //        if (Array.FindAll<GameObject>(
    //        Environments,
    //        x => x.GetComponent<RoadEnvironment>().type == environmentType
    //            &&
    //            x.GetComponent<RoadEnvironment>().classification == classification).Length == 0)
    //        {
    //            Debug.LogError("No block environments of type " + environmentType.ToString() + " and classification " + classification.ToString() + "found");
    //            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "No block environments of type " + environmentType.ToString() + " and classification " + classification.ToString() + "found");
    //        }
    //        return Array.FindAll<GameObject>(
    //        Environments,
    //        x => x.GetComponent<RoadEnvironment>().type == environmentType
    //            &&
    //            x.GetComponent<RoadEnvironment>().classification == classification);
    //    }
    //}

    public static void LoadResources()
    {
        Environments = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/Blocks/Env");
    }
    void Start()
    {
        if (environmentType == EnvironmentType.none)
            return;
        if (Environments == null || Environments.Length == 0)
        {
            Debug.LogError("No block environments loaded");
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "No block environments loaded");
            currentEnvironments = null;
        }
        currentEnvironments = Array.FindAll<GameObject>(
            Environments,
            x => x.GetComponent<RoadEnvironment>().type == environmentType
            &&
            x.GetComponent<RoadEnvironment>().classification == classification);
        if (currentEnvironments.Length == 0 || currentEnvironments == null)
        {
            Debug.LogError("No block environments of type " + environmentType.ToString() + " and classification " + classification.ToString() + " found");
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "No block environments of type " + environmentType.ToString() + " and classification " + classification.ToString() + "found");
        }
        else
            Show();
    }
    public void Show()
    {
        if (currentEnvironments != null)
            Instantiate(currentEnvironments[UnityEngine.Random.Range(0, currentEnvironments.Length)], transform);
    }
}
