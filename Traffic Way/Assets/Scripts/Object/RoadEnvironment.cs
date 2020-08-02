using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadEnvironment : MonoBehaviour
{
    public EnvironmentType type;
    public EnvironmentClassification classification;
}
public enum EnvironmentType { none, EnvDocks, EnvAirport, EnvVillage, EnvCity }
public enum EnvironmentClassification { leftBottomToRightTopWidth1, leftBottomToRightTopWidth2, leftBottomToRightTopWidth3, leftBottomToRightTopWidth4,
    leftTopToRightBottomWidth1, leftTopToRightBottomWidth2, leftTopToRightBottomWidth3, leftTopToRightBottomWidth4,
    centerToCenterWidth1, centerToCenterWidth2, centerToCenterWidth3, centerToCenterWidth4,
    oneCircle, twoCircles,
    finishRoad, startRoad, tripleSingleRoads, circleWithBoss}
