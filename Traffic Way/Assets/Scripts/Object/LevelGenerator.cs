﻿using System.Collections;
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    private static GameObject[] Crosses;
    private static GameObject[] Situations;
    private static GameObject[] Bosses;
    private static GameObject[] Puzzles;
    private static GameObject FinishBlock;
    private static GameObject FourCoins;
    private static GameObject FourCarsBeforeBoss;
    private static GameObject StartBlock;


    public static void LoadResources()
    {
        Crosses = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/Blocks/Crosses");
        Situations = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/Blocks/Situations");
        Bosses = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/Blocks/Bosses");
        Puzzles = Resources.LoadAll<GameObject>("TrafficWay/Prefabs/Blocks/Puzzles");
        FinishBlock = Resources.Load<GameObject>("TrafficWay/Prefabs/Blocks/Special/FinishRoad");
        FourCoins = Resources.Load<GameObject>("TrafficWay/Prefabs/Blocks/Special/FourCoins");
        FourCarsBeforeBoss = Resources.Load<GameObject>("TrafficWay/Prefabs/Blocks/Special/FourCarsBeforeBoss");
        StartBlock = Resources.Load<GameObject>("TrafficWay/Prefabs/Blocks/Special/Start");
        Debug.Log("Level blocks loaded. Crosses - " + Crosses.Length + ", Situations - " + Situations.Length + ", Bosses - " + Bosses.Length + ". Also " + FinishBlock.name + " and " + FourCoins.name + " are loaded");
    }

    public static bool isBossFignt
    {
        get;
        private set;
    }
    public static bool isPuzzle
    {
        get;
        private set;
    }
    private int lvlType;//Тип уровня по ТЗ https://docs.google.com/document/d/1Ue8eE5f6vBpleKTDr1nAHemjJJID7ihFEmhkrulEXqg/edit#heading=h.tbt6rb2j9cnk

    // Start is called before the first frame update
    void Start()
    {
        if (Engine.meta.currentRandomLevelBlocks == null || Engine.meta.currentRandomLevelBlocks.Count == 0)
        {
            isBossFignt = false;
            lvlType = Engine.actualLevel % 10;
            Debug.Log("Generator start. Level type - " + lvlType);
            Generate();
        }
        BuildSequence();
    }

    private void Generate()
    {
        //Create sequence
        List<string> BlockSequence = new List<string>();
        Logger.AddContent(UILogDataType.Level, "Autogenerated level type " + lvlType);
        switch (lvlType)
        {
            case 1:
                if (UnityEngine.Random.Range(0, 99) <= 49)
                    BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                else
                    BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);                
                break;
            case 2:
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);
                break;
            case 3:
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                break;
            case 4:
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(FourCarsBeforeBoss.name);
                BlockSequence.Add(Bosses[UnityEngine.Random.Range(0, Bosses.Length)].name);
                isBossFignt = true;
                break;
            case 5:
                float _r = UnityEngine.Random.Range(0, 99);
                if (_r <= 49)
                {
                    BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                    BlockSequence.Add(FourCoins.name);
                    BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                }
                else
                if(_r>= 50 && _r <= 79)
                {
                    BlockSequence.Add(Puzzles[UnityEngine.Random.Range(0, Puzzles.Length)].name);
                }
                else
                {
                    BlockSequence.Add(FourCoins.name);
                    BlockSequence.Add(FourCoins.name);
                    BlockSequence.Add(FourCoins.name);
                }
                break;
            case 6:
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(FourCarsBeforeBoss.name);
                BlockSequence.Add(Bosses[UnityEngine.Random.Range(0, Bosses.Length)].name);
                isBossFignt = true;
                break;
            case 7:
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                break;
            case 8:
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(FourCarsBeforeBoss.name);
                BlockSequence.Add(Bosses[UnityEngine.Random.Range(0, Bosses.Length)].name);
                isBossFignt = true;
                break;
            case 9:
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCoins.name);
                break;
            case 0:

                if (UnityEngine.Random.Range(0, 99) <= 69)
                {
                    BlockSequence.Add(FourCoins.name);
                    BlockSequence.Add(FourCoins.name);
                    BlockSequence.Add(FourCoins.name);
                }
                else
                {
                    BlockSequence.Add(Puzzles[UnityEngine.Random.Range(0, Puzzles.Length)].name);
                }
                break;
            default:
                Debug.LogError("Unknown lvl type " + lvlType);
                BlockSequence.Add(Crosses[UnityEngine.Random.Range(0, Crosses.Length)].name);
                BlockSequence.Add(Situations[UnityEngine.Random.Range(0, Situations.Length)].name);
                BlockSequence.Add(FourCarsBeforeBoss.name);
                BlockSequence.Add(Bosses[UnityEngine.Random.Range(0, Bosses.Length)].name);
                isBossFignt = true;
                BlockSequence.Add(FourCoins.name);
                break;
        }
        BlockSequence.Add(FinishBlock.name);

        Engine.meta.environmentType = (EnvironmentType)UnityEngine.Random.Range(1, 5);
        Engine.meta.currentRandomLevelBlocks = BlockSequence;

    }
    private void BuildSequence()
    {
        //Build sequence
        int lvlLength = 0;
        GameObject _currB;
        isBossFignt = false;
        isPuzzle = false;
        foreach (string _b in Engine.meta.currentRandomLevelBlocks)
        {
            GameObject _prefab;
            if (Array.Find<GameObject>(Crosses, x => x.name == _b) != null)
                _prefab = Array.Find<GameObject>(Crosses, x => x.name == _b);
            else
                if (Array.Find<GameObject>(Situations, x => x.name == _b) != null)
                    _prefab = Array.Find<GameObject>(Situations, x => x.name == _b);
                else
                    if (Array.Find<GameObject>(Bosses, x => x.name == _b) != null)
                    {
                        _prefab = Array.Find<GameObject>(Bosses, x => x.name == _b);
                        isBossFignt = true;
                    }
                    else
                        if (FinishBlock.name == _b)
                            _prefab = FinishBlock;
                        else
                            if (FourCoins.name == _b)
                                _prefab = FourCoins;
                            else
                                if (FourCarsBeforeBoss.name == _b)
                                    _prefab = FourCarsBeforeBoss;
                                else
                                    if(Array.Find<GameObject>(Puzzles, x => x.name == _b) != null)
                                    {
                                        isPuzzle = true;
                                        _prefab = Array.Find<GameObject>(Puzzles, x => x.name == _b);
                                       }
                                    else
                                    { 
                                        Debug.LogError("Unknown level block type " + _b + ". Used Finish block instead");
                                        GameAnalytics.NewErrorEvent(GAErrorSeverity.Warning, "Unknown level block type " + _b + ". Used Finish block instead");
                                        _prefab = FinishBlock;
                                    }
            
            _currB = Instantiate(_prefab, transform); 
            if (!isPuzzle)
            {
                _currB.GetComponent<Block>().environmentType = Engine.meta.environmentType;
                _currB.GetComponent<Block>().Show();
                _currB.transform.position = new Vector3(0f, lvlLength, 0f);
                lvlLength += _prefab.GetComponent<Block>().length;
            }
            _currB.name = _b;
        }
        if (isPuzzle)
            SceneManager.GetActiveScene().GetRootGameObjects()[2].SetActive(false);        
        else
        {
            _currB = Instantiate(StartBlock, transform);
            _currB.GetComponent<Block>().environmentType = Engine.meta.environmentType;
            _currB.GetComponent<Block>().Show();
            _currB.transform.position = new Vector3(0f, -37f, 0f);
            _currB.name = "Start block";
        }
        //If boss fight
        if (isBossFignt)
            CarDriver.CurrentCar.GetComponent<CarShooter>().enabled = true;
        Engine.Events.LevelGenerated();
    }
}