﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public static class Engine
{
    public static bool isVideoReady
    {
        get
        {
            return Advertisement.IsReady(PlacementType.video.ToString());
        }
    }
    public static bool isRewardedVideoReady
    {
        get
        {
            return Advertisement.IsReady(PlacementType.rewardedVideo.ToString());
        }
    }
    public static bool isBannerReady
    {
        get
        {
            return Advertisement.IsReady(PlacementType.banner.ToString());
        }
    }
    public static int actualLevel
    { get { return meta.passedLevels + 1; } }
    public static GameSessionState sessionState
    { get { return currentSession.state; } }
    public static bool paused
    {
        get
        {
            if (currentSession != null) 
                return currentSession.paused; 
            else
                return false;
        }
    }
    internal static GameData meta;
    private static int handcraftLevels
    {
        get
        {
            return SceneManager.sceneCountInBuildSettings - 2;
        }
    }
    private static LevelPlayingSession currentSession;

    public static void Load()
    {
        if (!Directory.Exists(Settings.savePath))
        {//Create directory if not exists
            Directory.CreateDirectory(Settings.savePath);
            Debug.Log("Saving path created " + Settings.savePath);
        }
        if (File.Exists(Settings.saveFile))
        {//load from file
            FileStream file = File.OpenRead(Settings.saveFile);
            StreamReader read = new StreamReader(file);
            string jsonData = read.ReadToEnd();
            int saveFileVersion = 0;
            if (jsonData.IndexOf("_version") > 0)
                saveFileVersion = Int32.Parse(jsonData.Substring(jsonData.IndexOf("_version") + 10, 1));
            else
            {
                Debug.LogError("Save file has no version");
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Critical, "Save file has no version");
                meta = new GameData();
            }
            if (saveFileVersion == GameData.Version)
            {
                meta = JsonUtility.FromJson<GameData>(jsonData);
                read.Close();
                file.Close();
                Debug.Log("Data loaded from " + Settings.saveFile);
            }
            else
            {
                Debug.LogError("Save file has an old verson " + saveFileVersion);
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Critical, "Save file has an old verson " + saveFileVersion);
                meta = new GameData();
            }
        }
        else
        {//load new game
            meta = new GameData();
        }
        Events.extraRewardReceived += Save;
    }
    private static void Save()
    {
        string jsonData = JsonUtility.ToJson(meta);
        FileStream file;
        if (File.Exists(Settings.saveFile))
            file = File.OpenWrite(Settings.saveFile);
        else
            file = File.Create(Settings.saveFile);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(jsonData);
        writer.Close();
        file.Close();
        Debug.Log(jsonData);
    }
    public static void ClearSaveFile()
    {
        File.Delete(Settings.saveFile);
        meta = new GameData();
        RestartLevel();
    }
    public static void Play()
    {
        SceneManager.activeSceneChanged += LevelChangeHandler;
        if (actualLevel <= handcraftLevels)
            SceneManager.LoadScene(actualLevel + 1);//Load handlecraft level
        else
            if (meta.lastHandcraftPassedLevel < handcraftLevels)
                SceneManager.LoadScene(meta.lastHandcraftPassedLevel + 1);//Load not played handcraft level
            else
                SceneManager.LoadScene(1);//Load autogenerated level
    }
    public static void LevelDone()
    {
        Save();
        if (isVideoReady&& !currentSession.ExtraRewardReceoved)
            Advertisement.Show(PlacementType.video.ToString());
        else
            SwitchLevel();
    }
    public static void LevelFailed()
    {
        if (isVideoReady)
            Advertisement.Show(PlacementType.video.ToString());
        else
            RestartLevel();
    }
    public static void Quit()
    {
        if (currentSession.state == GameSessionState.Won || currentSession.state == GameSessionState.Passed)
            Save();
        Application.Quit();
    }
    public static void SwitchPause()
    {
        if (currentSession.state != GameSessionState.Lost && currentSession.state != GameSessionState.Won)
            currentSession.paused = !currentSession.paused;
    }
    public static void AddCoins(int count)
    {
        meta.coinsCount += count;
    }
    internal static void RestartLevel()
    {
        Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    internal static void SwitchLevel()
    {
        if (currentSession.ExtraRewardReceoved)
            AddCoins(Settings.levelReward * Settings.extraRewardMultiplyer);
        else
            AddCoins(Settings.levelReward);
        int nextLevelBuildIndex;
        if (meta.lastHandcraftPassedLevel < handcraftLevels)
            nextLevelBuildIndex = meta.lastHandcraftPassedLevel + 2;
        else
            nextLevelBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (meta.passedLevels < handcraftLevels && !(SceneManager.GetActiveScene().buildIndex == 1))
            SceneManager.LoadScene(nextLevelBuildIndex);//Load handlecraft level
        else
            SceneManager.LoadScene(1);//Load autogenerated level
    }
    private static void LevelChangeHandler(Scene current, Scene next)
    {
        Advertisement.Load(PlacementType.video.ToString());
        Advertisement.Load(PlacementType.rewardedVideo.ToString());
        Advertisement.Load(PlacementType.banner.ToString());
        Debug.Log("Scene change detected. Current active scene is " + next.name);
        if (currentSession != null)//if current scene is not initial
            currentSession.Close(); //Closing previous session only if it existed
        else
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            //Loading ads if we just started playing
        }
        currentSession = new LevelPlayingSession(next);
        Logger.UpdateContent(UILogDataType.Level, next.name + ". Passed " + meta.passedLevels + " levels. Last passed handlecraft level: " + meta.lastHandcraftPassedLevel);
    }
    private class LevelPlayingSession: IUnityAdsListener
    {
        public Scene level { get; private set; }
        public GameSessionState state
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                if (value == GameSessionState.Passed || value == GameSessionState.Won)
                {
                    meta.passedLevels += 1;
                    if (level.buildIndex != 1)
                        meta.lastHandcraftPassedLevel += 1;
                }
                Engine.Events.GameSessionStateChanged(_state);
            }
        }
        private GameSessionState _state;
        public bool ExtraRewardReceoved;
        public bool paused;

        public LevelPlayingSession(Scene lvl)
        {
            level = lvl;
            state = GameSessionState.InProgress;
            paused = false;
            Advertisement.AddListener(this);
            Engine.Events.finishLineReached += FinishLineReachHandler;
            Engine.Events.crashHappened += OnCrashHappened;
        }        
        public void Close()
        {
            Advertisement.RemoveListener(this);
            Engine.Events.finishLineReached -= FinishLineReachHandler;
            Engine.Events.crashHappened -= OnCrashHappened;
        }
        public void FinishLineReachHandler()
        {
            Engine.SwitchPause();
            state = GameSessionState.Won;
        }
        public void OnCrashHappened()
        {
            state = GameSessionState.Lost;
        }
        public void OnUnityAdsReady(string placementId)
        {
            if (placementId == PlacementType.banner.ToString())
                Advertisement.Banner.Show(PlacementType.banner.ToString());
            Debug.Log(placementId + " ready");
        }
        public void OnUnityAdsDidError(string message)
        {
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, message);
        }
        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            Logger.UpdateContent(UILogDataType.Monetization, placementId + " " + showResult, true, true);
            if (placementId == PlacementType.rewardedVideo.ToString())
                if (showResult == ShowResult.Finished)
                {
                    ExtraRewardReceoved = true;
                    Events.ExtraRewardReceived();
                }
            if (placementId == PlacementType.video.ToString())
            {
                if (state == GameSessionState.Won)
                    SwitchLevel();
                if (state == GameSessionState.Lost)
                    RestartLevel();
            }
        }
        public void OnUnityAdsDidStart(string placementId)
        {
        }
    }
    internal class GameData
    {
        public const int Version = 1;
        [SerializeField]
        private int _version;
        public int passedLevels;
        public int lastHandcraftPassedLevel;
        public int coinsCount;
        public GameData()
        {
            _version = Version;
        }
    }
    internal class GameData_v0
    {
        public int passedLevels;
        public int lastHandcraftPassedLevel;
        public int coinsCount;
    }
    public static class Events
    {
        public delegate void GameStateHandler(GameSessionState state);
        public delegate void Fact();

        public static event Fact finishLineReached;
        public static event Fact crashHappened;
        public static event Fact extraRewardReceived;
        public static event Fact initialized;
        public static event GameStateHandler gameSessionStateChanged;

        public static void GameSessionStateChanged(GameSessionState state)
        {
            Debug.Log("Game session state change detected. New state: " + state.ToString());
            if (gameSessionStateChanged != null)
                gameSessionStateChanged(state);
        }
        public static void FinishLineReached()
        {
            Debug.Log("Finish line reached");
            if (finishLineReached != null)
                finishLineReached();
        }
        public static void ExtraRewardReceived()
        {
            Debug.Log("Extra reward received");
            if (extraRewardReceived != null)
                extraRewardReceived();
        }
        public static void CrashHappened()
        {
            Debug.Log("Crush happened");
            if (crashHappened != null)
                crashHappened();
        }
        public static void Initialized()
        {
            Debug.Log("Game engine initialized");
            if (initialized != null)
                initialized();
        }
    }
}
public enum GameSessionState { InProgress,Passed, Won, Lost }