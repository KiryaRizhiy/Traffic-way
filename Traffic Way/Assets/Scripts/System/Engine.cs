﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements;
//using GoogleMobileAds.Api;

public static class Engine
{
    //UNCOMMENT TO IMPLEMENT UNITY ADS
    //public static bool isVideoReady
    //{
    //    get
    //    {
    //        return Advertisement.IsReady(PlacementType.video.ToString());
    //    }
    //}
    //public static bool isRewardedVideoReady
    //{
    //    get
    //    {
    //        return Advertisement.IsReady(PlacementType.rewardedVideo.ToString());
    //    }
    //}
    //public static bool isBannerReady
    //{
    //    get
    //    {
    //        return Advertisement.IsReady(PlacementType.banner.ToString());
    //    }
    //}
    public static int actualLevel
    { get { return meta.passedLevels + 1; } }
    public static GameSessionState sessionState
    { get { return currentSession.state; } }
    public static bool isRewardedVideoReady
    {
        get
        {
            if (currentSession == null)
                return false;
            else
                return /*currentSession.adController*/AdMobController.isRewardedVideoReady;
        }
    }
    public static bool paused
    {
        get
        {
            if (currentSession != null) 
                return currentSession.paused; 
            else
                return true;
        }
    }
    public static bool newCarAppearenceReceivedInCurrentSession
    {
        get
        {
            if (currentSession == null)
                return false;
            else
                return currentSession.NewCarAppearenceReceived;
        }
    }
    public static bool extraRewardReceivedInCurrentSession
    {
        get
        {
            if (currentSession == null)
                return false;
            else
                return currentSession.ExtraRewardRequired;
        }
    }
    public static bool carTuningAvailable
    {
        get
        {
            return (
                meta.garage.GetCoinMaker(GarageCoinMakerType.MaterialsPedestal).level > 0
                ||
                meta.garage.GetCoinMaker(GarageCoinMakerType.MaterialsShelf).level > 0
                ||
                meta.garage.GetCoinMaker(GarageCoinMakerType.Workbench).level > 0
                );
        }
    }
    public static bool firstGarageUpgradeUnpacked
    {
        get
        {
            return
                (
                meta.garage.GetCoinMaker(GarageCoinMakerType.MaterialsPedestal).level
                +
                meta.garage.GetCoinMaker(GarageCoinMakerType.MaterialsShelf).level
                +
                meta.garage.GetCoinMaker(GarageCoinMakerType.Workbench).level
                ==
                1
                );
        }
    }
    public static bool isInPlayMode
    {
        get
        {
            return currentSession != null;
        }
    }
    public static float maxPossibleSpeed
    {
        get
        {
            int maxLevel = CoinMakersMaxUpdateLevels[Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsUpgradePrices[0], meta.garage.GetCarUpgrade(CarUpgradeType.Gearbox).correnspondentType.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length - 1];
            int rowNum = Array.FindAll<Char>(Regex.Replace(CarUpgradeSettingsUpgradeRates[0], CarUpgradeType.Gearbox.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
            int profit = Int32.Parse(CarUpgradeSettingsUpgradeRates[maxLevel + 1].Split(',')[rowNum]);
            return Settings.carSpeedLimit * (1 + profit / 100f) * Settings.carBoostMultiplyer;
        }
    }
    public static int rewardAmount
    {
        get
        {
            return currentSession.rewardAmount;
        }
    }
    public static List<Texture2D> CarsAppearences
    {
        get;
        private set;
    }
    public static List<Texture2D> CarAppearencesShadows
    {
        get;
        private set;
    }
    public static List<Texture2D> CarsAppearencesAngled
    {
        get;
        private set;
    }
    public static List<Texture2D> CarAppearencesShadowsAngled
    {
        get;
        private set;
    }
    public static bool initialized
    {
        get;
        private set;
    }
    internal static GameData meta;
    public static bool isBossFight
    {
        get
        {
            return CarDriver.CurrentCar.GetComponent<CarShooter>().isActiveAndEnabled;
        }
    }
    private static int totalHandcraftLevelsAmount
    {
        get
        {
            return SceneManager.sceneCountInBuildSettings - 2;
        }
    }
    private static LevelPlayingSession currentSession;
    private static String[] CoinMakersSettingsProfitRates;
    private static String[] CoinMakersSettingsUpgradePrices;
    private static String[] CoinMakersSettingsUnpackTime;
    private static String[] CarUpgradeSettingsUpgradePrices;
    private static String[] CarUpgradeSettingsUpgradeRates;
    private static List<Texture2D> CoinMakersTextures;
    private static List<int> CoinMakersMaxUpdateLevels;

    public static void Initialize()
    {
        Logger.AddContent(UILogDataType.Init, "Loading gamesave and initernal resources");
        try
        {
            Load();
        }
        catch (System.Exception e)
        {
            Logger.AddContent(UILogDataType.Init, e.Message);
            Logger.AddContent(UILogDataType.Init, "trace: " + e.StackTrace);
            Debug.LogError(e.Message);
        }
        Logger.AddContent(UILogDataType.Init, "Subscribing events");
        Subscribe();
        Logger.AddContent(UILogDataType.Init, "Loading resources");
        NPCCarDriver.LoadResources();
        CarShooter.LoadResources();
        TrafficLight.LoadResources();
        LevelGenerator.LoadResources();
        CarSelectInterface.LoadResources();
        Block.LoadResources();
        NPCCarController.LoadResources();
        RandomEmoji.LoadResources();
        CoinMaker.LoadResources();
        CarUpgradeInterface.LoadResources();
        //добавить cardriver.loadResources
        //Localization.LoadLocals(Application.systemLanguage);
        Localization.LoadLocals(SystemLanguage.English);
        initialized = true;
        Debug.Log("Max car speed is " + maxPossibleSpeed);
    }
    public static void InitializeTest()
    {
        if (currentSession == null)
            Initialize();
    }
    public static void ClearSaveFile()
    {
        File.Delete(Settings.saveFile);
        meta = new GameData();
        RestartLevel();
    }
    public static void CoinCollected()
    {
        AddCoins(1);
    }
    public static void Play()
    {
        if (actualLevel <= totalHandcraftLevelsAmount)
            SceneManager.LoadScene(actualLevel + 1);//Load handlecraft level
        else
            if (meta.lastHandcraftPassedLevel < totalHandcraftLevelsAmount)
                SceneManager.LoadScene(meta.lastHandcraftPassedLevel + 1);//Load not played handcraft level
            else
                SceneManager.LoadScene(1);//Load autogenerated level
    }
    public static void LevelDone()
    {
        meta.currentRandomLevelBlocks = null;
        Save();
        if (isBossFight)
        {
            meta.GarageOpened = true;
            Save();
            ToMainMenu();
        }
        else
            SwitchLevel();
    }
    public static void RestartLevel()
    {
        Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void ToMainMenu()
    {
        Save();
        currentSession.Close();
        currentSession = null;
        SceneManager.LoadScene(0);
    }
    public static void Quit()
    {
        meta.car.BoostOff();
        //if (currentSession.state == GameSessionState.Won || currentSession.state == GameSessionState.Passed)
        Save();
        Application.Quit();
    }
    public static void SwitchPause()
    {
        if (currentSession.state != GameSessionState.Lost && currentSession.state != GameSessionState.Won)
            currentSession.paused = !currentSession.paused;
    }
    public static void ShowRewardedVideo()
    {
        if (/*currentSession.adController*/AdMobController.isRewardedVideoReady)
            /*currentSession.adController*/AdMobController.ShowRewardedAd();
        else
            Engine.Events.AdNotReady(PlacementType.rewardedVideo);
    }
    public static CarAppearenceState GetAppearenceState(int appNum)
    {
        if (meta.car.unlockedAppearences.FindAll(x => x == appNum).Count > 0)
            return CarAppearenceState.Unlocked;
        if (meta.car.passedAppearences.FindAll(x => x == appNum).Count > 0)
            return CarAppearenceState.Passed;
        if (appNum < CarsAppearences.Count)
            return CarAppearenceState.Locked;
        return CarAppearenceState.Missing;
    }
    public static void AcceptGDPR()
    {
        meta.GDPRAccepted = true;
        Save();
    }
    public static void AddCoin()
    {
        meta.coinsCount++;
    }
    private static void AddCoins(int count)
    {
        meta.coinsCount += count;
    }
    private static void SwitchLevel()
    {
        AddCoins(currentSession.rewardAmount);
        int nextLevelBuildIndex;
        if (meta.lastHandcraftPassedLevel < totalHandcraftLevelsAmount)
            nextLevelBuildIndex = meta.lastHandcraftPassedLevel + 2;
        else
            nextLevelBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (meta.passedLevels < totalHandcraftLevelsAmount && !(SceneManager.GetActiveScene().buildIndex == 1))
            SceneManager.LoadScene(nextLevelBuildIndex);//Load handlecraft level
        else
            SceneManager.LoadScene(1);//Load autogenerated level
    }
    private static Texture2D GetCoinMakerTexture(GarageCoinMakerType type, int level)
    {
        int typeShift = 0;
        //Debug.Log("Loading texture for " + type.ToString() + " of level " + level);
        foreach (GarageCoinMakerType _t in Enum.GetValues(typeof(GarageCoinMakerType)))
        {
            if (_t != type)
                typeShift += CoinMakersMaxUpdateLevels[Array.IndexOf(Enum.GetValues(typeof(GarageCoinMakerType)), _t)] + 1;
            else
            {
                //Debug.Log("Texture load shift " + typeShift);
                return CoinMakersTextures[typeShift + level];
            }
        }
        return null;
    }
    private static void Save()
    {
        string jsonData = JsonUtility.ToJson(meta);
        File.WriteAllText(Settings.saveFile, jsonData);
        Debug.Log(jsonData);
    }
    private static void Load()
    {
        //Loading resources
        TextAsset _txt;
        _txt = Resources.Load<TextAsset>("TrafficWay/Other/CoinMakersSettingsProfitRates");
        CoinMakersSettingsProfitRates = _txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None);
        _txt = Resources.Load<TextAsset>("TrafficWay/Other/CoinMakersSettingsUpgradePrices");
        CoinMakersSettingsUpgradePrices = new String[_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None).Length];
        Array.Copy(_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None), CoinMakersSettingsUpgradePrices, CoinMakersSettingsUpgradePrices.Length);
        _txt = Resources.Load<TextAsset>("TrafficWay/Other/CoinMakersSettingsUnpackTime");
        CoinMakersSettingsUnpackTime = new String[_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None).Length];
        Array.Copy(_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None), CoinMakersSettingsUnpackTime, CoinMakersSettingsUnpackTime.Length);

        _txt = Resources.Load<TextAsset>("TrafficWay/Other/CarUpgradeSettingsUpgradePrices");
        CarUpgradeSettingsUpgradePrices = new String[_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None).Length];
        Array.Copy(_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None), CarUpgradeSettingsUpgradePrices, CarUpgradeSettingsUpgradePrices.Length);
        _txt = Resources.Load<TextAsset>("TrafficWay/Other/CarUpgradeSettingsUpgradeRates");
        CarUpgradeSettingsUpgradeRates = new String[_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None).Length];
        Array.Copy(_txt.text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None), CarUpgradeSettingsUpgradeRates, CarUpgradeSettingsUpgradeRates.Length);

        CoinMakersMaxUpdateLevels = new List<int>();
        CoinMakersTextures = new List<Texture2D>();
        foreach (GarageCoinMakerType _t in Enum.GetValues(typeof(GarageCoinMakerType)))
        {
            int previousCount = CoinMakersTextures.Count;
            CoinMakersTextures.AddRange(Resources.LoadAll<Texture2D>("TrafficWay/Textures/Garage/"+_t.ToString()+"/"));
            CoinMakersMaxUpdateLevels.Add(CoinMakersTextures.Count - previousCount - 1);
            Debug.Log("Loaded textures of " + _t.ToString());
        }

        CarsAppearences = new List<Texture2D>();
        CarAppearencesShadows = new List<Texture2D>();
        CarsAppearencesAngled = new List<Texture2D>();
        CarAppearencesShadowsAngled = new List<Texture2D>();
        int _carsAmt = Resources.LoadAll<Texture2D>("TrafficWay/Textures/PlayerCar").Length / 4;
        Debug.Log("Cars amount: " + _carsAmt);
        for (int i = 0; i < _carsAmt; i++)
        {
            CarsAppearences.Add(Resources.Load<Texture2D>("TrafficWay/Textures/PlayerCar/" + i.ToString() + "_Car"));
            CarAppearencesShadows.Add(Resources.Load<Texture2D>("TrafficWay/Textures/PlayerCar/" + i.ToString() + "_Shadow"));
            CarsAppearencesAngled.Add(Resources.Load<Texture2D>("TrafficWay/Textures/PlayerCar/" + i.ToString() + "_Car_Angled"));
            CarAppearencesShadowsAngled.Add(Resources.Load<Texture2D>("TrafficWay/Textures/PlayerCar/" + i.ToString() + "_Shadow_Angled"));
        }
        Debug.Log("Loaded " + CarsAppearences.Count + " cars and " + CarAppearencesShadows.Count + " shadows");

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
            Debug.Log("Data loaded from " + Settings.saveFile);
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
        meta.garage.Initialize();
    }
    private static void Subscribe()
    {
        Events.extraRewardReceived += Save;
        Events.finishLineReached += HandleFinishLineCrossed;
        Events.passLineReached += HandlePassLineReach;
        Events.crashHappened += HandleChashHappened;
        Events.shieldDestroyed += HandleShieldDestroy;
        Events.adFinished += OnAdFinished;
        SceneManager.activeSceneChanged += OnLevelChanged;
    }
    private static void OnLevelChanged(Scene current, Scene next)
    {
        //UNCOMMENT TO IMPLEMENT UNITY ADS
        //Advertisement.Load(PlacementType.video.ToString());
        //Advertisement.Load(PlacementType.rewardedVideo.ToString());
        //Advertisement.Load(PlacementType.banner.ToString());
        Debug.Log("Scene change detected. Current active scene is " + next.name);
        if (currentSession != null)//if current scene is not initial
            currentSession.Close(); //Closing previous session only if it existed
        //else
        //{
        //    Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        //    //Loading ads if we just started playing
        //}
        currentSession = new LevelPlayingSession(next);
        Logger.UpdateContent(UILogDataType.Level, next.name + ". Passed " + meta.passedLevels + " levels. Last passed handlecraft level: " + meta.lastHandcraftPassedLevel);
    }
    private static void HandleFinishLineCrossed()
    {
        currentSession.state = GameSessionState.Won;
        meta.car.ProgressReached();
        currentSession.paused = true;
        meta.car.BoostOff();
        //if (currentSession.adController.isRegularVideoReady)
        if (AdMobController.isRegularVideoReady)
            /*currentSession.adController*/AdMobController.ShowRegularAd();
        else
            Engine.Events.AdNotReady(PlacementType.interstitial);
    }
    private static void HandlePassLineReach()
    {
        currentSession.state = GameSessionState.Passed;
    }
    private static void HandleChashHappened()
    {
        currentSession.state = GameSessionState.Lost;
        meta.car.BoostOff();
        //currentSession.paused = true;
        if (/*currentSession.adController*/AdMobController.isRegularVideoReady)
            /*currentSession.adController*/AdMobController.ShowRegularAd();
        else
            Engine.Events.AdNotReady(PlacementType.interstitial);
    }
    private static void HandleShieldDestroy()
    {
        meta.car.RemoveShield();
    }
    private static void OnAdFinished(PlacementType type)
    {
        if (type == PlacementType.rewardedVideo && currentSession != null)
        {
            if (meta.car.previousNextAppearenceProgress > 0 && meta.car.nextAppearenceProgress == 0 && !currentSession.NewCarAppearenceReceived)
            {
                if (meta.car.nextPassedAppearenceNum != -1)
                    meta.car.UnlockAppearence(meta.car.nextPassedAppearenceNum - 1, true);
                else
                    meta.car.UnlockAppearence(CarsAppearences.Count - 1);
                currentSession.NewCarAppearenceReceived = true;
                Events.NewCarAppearenceReceived();
            }
            else
            {
                currentSession.ExtraRewardRequired = true;
                Events.ExtraRewardReceived();
            }
        }
    }
    private class LevelPlayingSession/*: IUnityAdsListener UNCOMMENT TO IMPLEMENT UNITY ADS*/
    {
        public Scene level 
        { get; private set; }
        public GameSessionState state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                Debug.Log("State changed");
                if (value == GameSessionState.Won)
                {
                    meta.passedLevels += 1;
                    if (level.buildIndex != 1)
                        meta.lastHandcraftPassedLevel += 1;
                }
                switch (value)
                {
                    case GameSessionState.Won:
                        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Traffic way", level.name, "Level progress", actualLevel);
                        break;
                    case GameSessionState.Lost:
                        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Traffic way", level.name, "Level progress", actualLevel);
                        break;
                }
                Engine.Events.GameSessionStateChanged(_state);
            }
        }
        public AdMobController adController
        { get; private set; }
        public int rewardAmount
        {
            get
            {
                if (ExtraRewardRequired)
                    return Settings.levelReward * Settings.extraRewardMultiplyer;
                else
                    return Settings.levelReward;
            }
        }
        public bool ExtraRewardRequired;
        public bool NewCarAppearenceReceived;
        public bool paused
        {
            get
            { 
                return _paused; 
            }
            set
            {
                if (value == _paused)
                    return;
                if (!_paused)
                {
                    _paused = value;
                    Engine.Events.Paused();
                }
                else
                {
                    _paused = value;
                    Engine.Events.Unpaused();
                }
            }
        }
        public bool bossFight
        {
            get;
            private set;
        }

        private bool _paused;
        private GameSessionState _state;

        public LevelPlayingSession(Scene lvl)
        {
            level = lvl;
            state = GameSessionState.InProgress;
            paused = false;
            adController = new GameObject().AddComponent<AdMobController>();
            adController.gameObject.name = "AdMobController";
            ////Advertisement.AddListener(this); UNCOMMENT TO IMPLEMENT UNITY ADS
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,"Traffic way", level.name,"Level progress",actualLevel);
        }
        public void Close()
        {
            //meta.car.BoostOff();
            //Advertisement.RemoveListener(this); UNCOMMENT TO IMPLEMENT UNITY ADS
        }

        //UNCOMMENT TO IMPLEMENT UNITY ADS
        //public void OnUnityAdsReady(string placementId)
        //{
        //    if (placementId == PlacementType.banner.ToString())
        //        Advertisement.Banner.Show(PlacementType.banner.ToString());
        //    Debug.Log(placementId + " ready");
        //}
        //public void OnUnityAdsDidError(string message)
        //{
        //    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, message);
        //}
        //public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        //{
        //    Logger.UpdateContent(UILogDataType.Monetization, placementId + " " + showResult, true, true);
        //    if (placementId == PlacementType.rewardedVideo.ToString())
        //        if (showResult == ShowResult.Finished)
        //        {
        //            ExtraRewardReceoved = true;
        //            Events.ExtraRewardReceived();
        //        }
        //    if (placementId == PlacementType.video.ToString())
        //    {
        //        if (state == GameSessionState.Won)
        //            SwitchLevel();
        //        if (state == GameSessionState.Lost)
        //            RestartLevel();
        //    }
        //}
        //public void OnUnityAdsDidStart(string placementId)
        //{
        //}
    }
    #region Metadata models versions    
    internal class GameData
    {
        public const int Version = 0;
        [SerializeField]
        private int _version;
        public int passedLevels;
        public int lastHandcraftPassedLevel;
        public int coinsCount
        {
            get 
            { 
                return _coinsCount;
            }
            set 
            {
                int diff = value - _coinsCount;
                    _coinsCount = value;
                if (diff>0)//Increase coins amount                
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", diff,"Coin","Coin");                
                else
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Coin", -diff, "Coin", "Coin");  
            }
        }
        [SerializeField]
        private int _coinsCount;
        [SerializeField]
        private List<string> _currentRandomLevelBlocks;
        [SerializeField]
        private EnvironmentType _environmentType;
        public List<string> currentRandomLevelBlocks
        {
            get
            {
                return _currentRandomLevelBlocks;
            }
            set
            {
                _currentRandomLevelBlocks = value;
                Save();
            }
        }
        public EnvironmentType environmentType
        {
            get
            {
                return _environmentType;
            }
            set
            {
                _environmentType = value;
                Save();
            }
        }
        public bool GDPRAccepted;
        public bool GarageOpened;
        public bool FirstUpgradeNotPurchased
        {
            get
            {
                return meta.garage.GetCoinMaker(GarageCoinMakerType.ToolsWall).level == 0;
            }
        }
        public GarageData garage;
        [SerializeField]
        public CarData car;
        public GameData()
        {
            _version = Version;
            garage = new GarageData();
            car = new CarData();
            //if (Settings.testMode)
            //    coinsCount = 5000;
            GDPRAccepted = false;
            GarageOpened = false;
        }

        [Serializable]
        public class CarData
        {
            public bool isBoosted;
            public List<int> unlockedAppearences;
            public List<int> passedAppearences;
            private List<CarUpgradeData> upgrades;
            public int currentAppearenceNum
            {
                get {return _currentAppearenceNum; }
                set
                {
                    _currentAppearenceNum = value;
                    Events.CarAppearenceChanged();
                }
            }
            public int nextAppearenceProgress;
            public int previousNextAppearenceProgress;
            public int availableNitroBottles
            {
                get
                {
                    return _availableNitroBottles;
                }
            }
            public NitroState nitroState
            {
                get
                {
                    if (meta.garage.GetCoinMaker(GarageCoinMakerType.ToolsWall).level == 0)
                        return NitroState.Blocked;
                    else
                    {
                        if (!nitroPurchased)
                            return NitroState.NotPurchased;
                        else
                        {
                            if (_availableNitroBottles == 0)
                                return NitroState.Empty;
                            else
                                return NitroState.Available;
                        }
                    }
                }
            }

            [SerializeField]
            private int _availableNitroBottles;
            [SerializeField]
            private bool nitroPurchased;
            [SerializeField]
            private int _currentAppearenceNum;
            public bool hasShield
            {
                get;
                private set;
            }
            public float speedLimit
            {
                get
                {
                    if (isBoosted)
                        return Settings.carSpeedLimit * Settings.carBoostMultiplyer;
                    else
                        return Settings.carSpeedLimit;
                }
            }
            public float acceleration
            {
                get
                {
                    if (isBoosted)
                        return Settings.carAcceleration * Settings.carBoostMultiplyer;
                    else
                        return Settings.carAcceleration;
                }
            }
            public float brakingSpeed
            {
                get
                {
                    if (isBoosted)
                        return Settings.carBraking * Settings.carBoostMultiplyer;
                    else
                        return Settings.carBraking;
                }
            }
            public int nextPassedAppearenceNum
            {
                get
                {
                    if (passedAppearences.Max() < CarsAppearences.Count - 1)
                        return passedAppearences.Max() + 1;
                    else
                        return -1;
                }
            }
            public CarUpgradeData GetCoinMaker(CarUpgradeType Type)
            {
                return upgrades.Find(x => x.type == Type);
            }
            public void BoostOn()
            {
                isBoosted = true;
                hasShield = true;
                _availableNitroBottles--;
                Save();
                GameAnalytics.NewDesignEvent("Meta:Car:Boosted");
                Engine.Events.NitroActivated();
            }
            public void BoostOff()
            {
                isBoosted = false;
                hasShield = false;
            }
            public void RemoveShield()
            {
                hasShield = false;
                GameAnalytics.NewDesignEvent("Meta:Car:Shield destroyed");
            }
            public void UnlockAppearence(int appNum,bool setAsCurrent = false)
            {
                unlockedAppearences.Add(appNum);
                if (setAsCurrent)
                    SwitchCurrentAppearenceTo(appNum);
                GameAnalytics.NewDesignEvent("Meta:Car:UnlkAppearence_" + Engine.CarsAppearences[appNum].name);
            }
            public void SwitchCurrentAppearenceTo(int appNum)
            {
                GameAnalytics.NewDesignEvent("Meta:Car:Activated_appearence_" + appNum.ToString());
                currentAppearenceNum = appNum;
            }
            public void ProgressReached()
            {
                if (nextPassedAppearenceNum == -1)
                {
                    previousNextAppearenceProgress = -1;
                    nextAppearenceProgress = -1;
                    return;
                }
                previousNextAppearenceProgress = nextAppearenceProgress;
                nextAppearenceProgress += Settings.levelCarProgress;
                if (nextAppearenceProgress >= 100)
                {
                    passedAppearences.Add(nextPassedAppearenceNum);
                    GameAnalytics.NewDesignEvent("Meta:Car:PssAppearence_" + Engine.CarsAppearences[passedAppearences.Max()].name); 
                    nextAppearenceProgress = 0;
                }
            }
            public void PurchaseNitro()
            {
                if(meta.coinsCount < Settings.nitroCost)
                {
                    Debug.LogError("Trying to buy a nitro with unenough coins");
                    return;
                }
                nitroPurchased = true;
                meta._coinsCount -= Settings.nitroCost;
                Save();
                Engine.Events.GarageStateChanged(GarageCoinMakerType.ToolsWall);
            }
            public void AddBaloon()
            {
                _availableNitroBottles += 1;
                Save();
            }

            public CarData()
            {
                BoostOff();
                unlockedAppearences = new List<int> { 0 };
                passedAppearences = new List<int> { 0 };
                SwitchCurrentAppearenceTo(0);
                nextAppearenceProgress = 0;
                previousNextAppearenceProgress = 0;
                nitroPurchased = false;
                _availableNitroBottles = 1;
                upgrades = new List<CarUpgradeData>();
                foreach (CarUpgradeType _t in Enum.GetValues(typeof(CarUpgradeType)))
                {
                    upgrades.Add(new CarUpgradeData(_t));
                }
            }
        }
        [Serializable]
        public class GarageData
        {
            [SerializeField]
            private List<CoinMakerData> CoinMakers;
            [SerializeField]
            private List<CarUpgradeData> CarUpgrades;
            public long lastTVWatched;
            public TVSetState tvSetState
            {
                get
                {
                    //    Logger.UpdateContent(UILogDataType.GameState, "Video ready: " + Engine.isRewardedVideoReady);
                    //    Logger.AddContent(UILogDataType.GameState, "TV cooldown passed: " + (DateTime.UtcNow - new DateTime(lastTVWatched) >= new TimeSpan(0,Settings.TVCooldownMinutes,0)));
                    if (DateTime.UtcNow - new DateTime(lastTVWatched) >= new TimeSpan(0, Settings.TVCooldownMinutes, 0))
                    {
                        if (AdMobController.isRewardedVideoReady)
                            return TVSetState.ReadyToWatch;
                        else
                            return TVSetState.VideoNotReady;
                    }
                    else
                        return TVSetState.RecentlyWatched;
                }
            }
            public GarageData()
            {
                CoinMakers = new List<CoinMakerData>();
                foreach (GarageCoinMakerType _t in Enum.GetValues(typeof(GarageCoinMakerType)))
                {
                    CoinMakers.Add(new CoinMakerData(_t));
                }
                CarUpgrades = new List<CarUpgradeData>();
                foreach (CarUpgradeType _t in Enum.GetValues(typeof(CarUpgradeType)))
                {
                    CarUpgrades.Add(new CarUpgradeData(_t));
                }
                lastTVWatched = DateTime.UtcNow.Ticks;
            }
            public CoinMakerData GetCoinMaker(GarageCoinMakerType Type)
            {
                return CoinMakers.Find(x=>x.type == Type);
            }
            public CarUpgradeData GetCarUpgrade(CarUpgradeType Type)
            {
                return CarUpgrades.Find(x => x.type == Type);
            }
            public void TVWatched()
            {
                lastTVWatched = DateTime.UtcNow.Ticks;
                AddCoins(Settings.TVWatchReward);
                Save();
                GameAnalytics.NewDesignEvent("Meta:Garage:TV_Watched");
                RegisterTVSetTimeEvent();
            }
            public void Initialize()
            {
                foreach (CoinMakerData _cmd in CoinMakers)
                    _cmd.GenerateTimeEvents();
                if(tvSetState == TVSetState.RecentlyWatched)
                {
                    RegisterTVSetTimeEvent();
                }
            }
            private void RegisterTVSetTimeEvent()
            {
                TimeEventsManager.RegisterTimeEvent("tvSetReady", lastTVWatched + new TimeSpan(0, Settings.TVCooldownMinutes, 0).Ticks);
            }
        }
        [Serializable]
        public class CoinMakerData
        {
            private enum CoinMakerInternalStatuses { NotPurchased, Normal, Unpacking};
            public GarageCoinMakerType type;
            public int level;
            public long lastCoinCollect;
            public long unpackStartTicks;
            private long unpackFinishTicks
            {
                get
                {
                    return (unpackStartTicks + unpackSeconds * TimeSpan.TicksPerSecond);
                }
            }
            public DateTime unpackStartTime
            {
                get
                {
                    return new DateTime(unpackStartTicks);
                }
            }
            public TimeSpan unpackTimeLeft
            {
                get
                {
                    return new DateTime(unpackStartTicks + unpackSeconds * TimeSpan.TicksPerSecond) - DateTime.UtcNow;
                }
            }
            public CoinMakerStates state
            {
                get
                {
                    if (type != GarageCoinMakerType.ToolsWall && meta.garage.GetCoinMaker(GarageCoinMakerType.ToolsWall).level == 0)
                        return CoinMakerStates.Blocked;
                    else
                        if (internalState == CoinMakerInternalStatuses.Normal)
                            return CoinMakerStates.Normal;
                        else
                            if (internalState == CoinMakerInternalStatuses.Unpacking)
                            {
                                if ((DateTime.UtcNow - unpackStartTime).TotalSeconds < unpackSeconds)
                                    return CoinMakerStates.Unpacking;
                                else
                                    return CoinMakerStates.UnpackingFinished;
                            }
                        else
                            if (updatable)
                                return CoinMakerStates.NotPurchased;
                            else
                                return CoinMakerStates.MaxLevelReached;
                }
            }
            [SerializeField]
            private CoinMakerInternalStatuses internalState;
            public Texture2D currentTexture
            {
                get
                {
                    return Engine.GetCoinMakerTexture(type, level);
                }
            }
            private bool updatable
            {
                get
                {
                    return level < CoinMakersMaxUpdateLevels[Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsUpgradePrices[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length - 1];
                }
            }
            public Texture2D futureTexture
            {
                get
                {
                    if (updatable)
                        return Engine.GetCoinMakerTexture(type, level + 1);
                    else
                        return null;
                }
            }
            public int updatePrice
            {
                get
                {
                    int rowNum = Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsUpgradePrices[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
                    int price = Int32.Parse(CoinMakersSettingsUpgradePrices[level + 1].Split(',')[rowNum]);
                    return price;
                }
            }
            public int unpackSeconds
            {
                get
                {
                    int rowNum = Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsUnpackTime[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
                    int time = Int32.Parse(CoinMakersSettingsUnpackTime[level + 1].Split(',')[rowNum]);
                    return time;
                }
            }
            public int profitRate
            {
                get
                {
                    int rowNum = Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsProfitRates[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
                    int profit = Int32.Parse(CoinMakersSettingsProfitRates[level + 1].Split(',')[rowNum]);
                    return profit;
                }
            }
            public int currentProfit
            {
                get
                {
                    TimeSpan t = DateTime.UtcNow - new DateTime(lastCoinCollect);
                    //Debug.Log("Last coin collect is " + new DateTime(lastCoinCollect).ToString() + " now " + DateTime.UtcNow + ". Difference in minutes - " + t.TotalMinutes);
                    int ticks = Convert.ToInt32(Math.Floor(t.TotalMinutes / Settings.coinMakerTickMinutes));
                    if (ticks > Settings.paidTicksLimit)
                        ticks = Settings.paidTicksLimit;
                    //Debug.Log("Last coin collected : " + (new DateTime(lastCoinCollect)).ToString() + " time interval: " + t.ToString() + " tickMinutes: " + Settings.coinMakerTickMinutes + " ticks:" + ticks + " total profit:" + (ticks*profitRate));
                    return ticks * profitRate;
                }
            }
            public string description
            {
                get
                {
                    switch (type)
                    {
                        case GarageCoinMakerType.MaterialsPedestal:
                            return level==0 ? "Earns coins. Allows to upgrade car brakes" : "Earn more coins and increase brakes max level";
                        case GarageCoinMakerType.Workbench:
                            return level == 0 ? "Earns coins. Allows to upgrade gearbox" : "Earn more coins and increase gearbox max level";
                        case GarageCoinMakerType.MaterialsShelf:
                            return level == 0 ? "Earns coins. Allows to upgrade engine" : "Earn more coins and increase engine max level";
                        case GarageCoinMakerType.ToolsWall:
                            return level == 0 ? "Earns coins and unlocks other garage upgrades" : "Earn more coins. More effective than other upgrades";
                        default:
                            Debug.LogError("Attempt to read description of unknown type garage element " + type.ToString());
                            return "Unknown garage element";
                    }
                }
            }
            public CoinMakerData(GarageCoinMakerType Type)
            {
                type = Type;
                level = 0;
                lastCoinCollect = DateTime.UtcNow.Ticks;
                unpackStartTicks = DateTime.UtcNow.Ticks;
                if (type != GarageCoinMakerType.ToolsWall)
                    internalState = CoinMakerInternalStatuses.NotPurchased;
                else
                    internalState = CoinMakerInternalStatuses.Unpacking;
            }
            public int StartUnpacking()
            {
                if (meta.coinsCount < updatePrice)
                {
                    Debug.LogError("Invalid purchase attempt. Trying to purchase update for " + type.ToString() + ". It's cost is " + updatePrice + " coins, but there is only " + meta.coinsCount + " coins");
                    return -1;
                }
                unpackStartTicks = DateTime.UtcNow.Ticks;
                internalState = CoinMakerInternalStatuses.Unpacking;
                meta._coinsCount -= updatePrice;
                Save();
                RegisterUnpackFinishEvent();
                Engine.Events.GarageStateChanged(type);

                return unpackSeconds;
            }
            public void Unpack()
            {
                if (internalState == CoinMakerInternalStatuses.Unpacking)
                {
                    level += 1;
                    internalState = CoinMakerInternalStatuses.Normal;
                    if (level == 1)
                        lastCoinCollect = DateTime.UtcNow.Ticks;
                    Engine.Events.GarageStateChanged(type);
                    Save();
                    GameAnalytics.NewDesignEvent("Meta:Garage:Upg_" + type.ToString() + "_" + level.ToString());
                }
                else
                    Debug.LogError("Attemt to finish unpacking coin maker " + type.ToString() + ", that din't start unpacking");
            }
            public void DecreaseUnpackTimeByHour()
            {
                unpackStartTicks -= TimeSpan.TicksPerHour;
                Save();
                UpdateUnpackFinishEvent(TimeSpan.TicksPerHour);
            }
            public void GenerateTimeEvents()
            {
                if (internalState == CoinMakerInternalStatuses.Unpacking)
                    if (unpackFinishTicks > DateTime.UtcNow.Ticks)
                        RegisterUnpackFinishEvent();
            }
            public void CollectACoin()
            {
                if (currentProfit > 0)
                {
                    int _newTicksLeft = currentProfit/profitRate;
                    lastCoinCollect = DateTime.UtcNow.Ticks - (_newTicksLeft - 1) * Settings.coinMakerTickMinutes * TimeSpan.TicksPerMinute - 1;
                    Debug.Log("New last coin collect time: " + (new DateTime(lastCoinCollect)).ToString()
                        + " offset is" + (new TimeSpan((_newTicksLeft - 1) * Settings.coinMakerTickMinutes * TimeSpan.TicksPerMinute - 1)).ToString());
                    Debug.Log(type.ToString() + " coins left: " + currentProfit);
                    AddCoins(1);
                    Save();
                    Engine.Events.GarageStateChanged(type);
                }
            }
            private void RegisterUnpackFinishEvent()
            {
                TimeEventsManager.RegisterTimeEvent(type.ToString() + "_unpackFinish", unpackFinishTicks);
            }
            private void UpdateUnpackFinishEvent(long offset)
            {
                TimeEventsManager.UpdateTimeEvent(type.ToString() + "_unpackFinish", unpackFinishTicks - offset, unpackFinishTicks);
            }
            public override string ToString()
            {
                switch (type)
                {
                    case GarageCoinMakerType.MaterialsPedestal:
                        return "Materials pedestal";
                    case GarageCoinMakerType.MaterialsShelf:
                        return "Materials shelf";
                    case GarageCoinMakerType.Workbench:
                        return "Workbench";
                    default:
                        return "Tools wall";
                }
            }
        }
        [Serializable]
        public class CarUpgradeData
        {
            public int level;
            public bool unlockDemonstrated;
            public int updatePrice
            {
                get
                {
                    int rowNum = Array.FindAll<Char>(Regex.Replace(CarUpgradeSettingsUpgradePrices[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
                    int price = Int32.Parse(CarUpgradeSettingsUpgradePrices[level + 1].Split(',')[rowNum]);
                    return price;
                }
            }
            public float upgradeRate
            {
                get
                {
                    int rowNum = Array.FindAll<Char>(Regex.Replace(CarUpgradeSettingsUpgradeRates[0], type.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length;
                    int profit = Int32.Parse(CarUpgradeSettingsUpgradeRates[level + 1].Split(',')[rowNum]);
                    return 1 + profit/100f;
                }
            }
            public CarUpgradeType type;
            private bool ugradable
            {
                get
                {
                    return level < CoinMakersMaxUpdateLevels[Array.FindAll<Char>(Regex.Replace(CoinMakersSettingsUpgradePrices[0], correnspondentType.ToString() + "{1}.*", String.Empty, RegexOptions.IgnoreCase).ToCharArray(), x => x == ',').Length - 1];
                }
            }
            public GarageCoinMakerType correnspondentType
            {
                get
                {
                    switch (type)
                    {
                        case (CarUpgradeType.Gearbox):
                            return GarageCoinMakerType.Workbench;
                        case (CarUpgradeType.Brakes):
                            return GarageCoinMakerType.MaterialsPedestal;
                        default:
                            return GarageCoinMakerType.MaterialsShelf;
                    }
                }
            }
            public CoinMakerData correspondentCoinMaker
            {
                get
                {
                    return meta.garage.GetCoinMaker(correnspondentType);
                }
            }
            public CarUpgradeState state
            {
                get
                {   if (!ugradable)
                        return CarUpgradeState.TopLevelReached;
                    else
                        if (correspondentCoinMaker.level == level)
                        return CarUpgradeState.Blocked;
                    else
                        return CarUpgradeState.Normal;
                }
            }
            public CarUpgradeData(CarUpgradeType Type)
            {
                level = 0;
                type = Type;
                unlockDemonstrated = false;
            }
            public void Upgrade()
            {
                if (!ugradable)
                {
                    Debug.LogError("Trying to upgrade top leveled car upgrade " + type.ToString() + ". Current level: " + level);
                    return;
                }
                if (updatePrice > meta.coinsCount)
                {
                    Debug.LogWarning("Not enough coins to upgrade " + type.ToString() + ". Current coins: " + meta.coinsCount + ", upgrade cost: " + updatePrice);
                    return;
                }
                meta._coinsCount -= updatePrice;
                level += 1;
                if (state == CarUpgradeState.Blocked)
                {
                    unlockDemonstrated = false;
                    Debug.LogWarning("Unlock demonstration flag dropped");
                }
                Save();
                Engine.Events.CarUpgradeStateChanged(type);
            }
            public void UnlockDemonstrated()
            {
                unlockDemonstrated = true;
                Debug.LogWarning("Unlock demonstrated");
            }
            public override string ToString()
            {
                switch (type)
                {
                    case CarUpgradeType.Engine:
                        return "Engine improves acceleration";
                    case CarUpgradeType.Brakes:
                        return "Car brakes for faster slow down";
                    default:
                        return "Gearbox increases speed limit";
                }    
            }
        }
    }
    #endregion
    public static class Events
    {
        public delegate void GameStateHandler(GameSessionState state);
        public delegate void Fact();
        public delegate void AdsInfo(PlacementType type);
        public delegate void ZoneReach(GameObject zone);
        public delegate void GarageStateChange(GarageCoinMakerType type);
        public delegate void CarUpgradeStateChange(CarUpgradeType Type);
        public delegate void TimeEvent(string eventName);

        public static event Fact finishLineReached;
        public static event Fact passLineReached;
        public static event Fact crashHappened;
        public static event Fact shieldDestroyed;
        public static event Fact extraRewardReceived;
        public static event Fact newCarAppearenceReceived;
        public static event Fact carAppearenceChanged;
        public static event Fact initialized;
        public static event Fact levelGenerated;
        public static event Fact paused;
        public static event Fact unpaused;
        public static event Fact nitroActivated;
        public static event TimeEvent timeEventOccured;
        public static event ZoneReach zoneReached;
        public static event ZoneReach zoneLeft;
        public static event AdsInfo adLoaded;
        public static event AdsInfo adNotReady;
        public static event AdsInfo adFinished;
        public static event AdsInfo adSkipped;
        public static event AdsInfo adFailed;
        public static event AdsInfo adOpened;
        public static event AdsInfo adUserLeave;
        public static event GameStateHandler gameSessionStateChanged;
        public static event GarageStateChange onGarageStateChanged;
        public static event CarUpgradeStateChange onCarUpgradeStateChanged;

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
        public static void PassLineReached()
        {
            Debug.Log("Pass line reached");
            if (passLineReached != null)
                passLineReached();
        }
        public static void NewCarAppearenceReceived()
        {
            Debug.Log("New car appearence received");
            if (newCarAppearenceReceived != null)
                newCarAppearenceReceived();
        }
        public static void CarAppearenceChanged()
        {
            Debug.Log("Car appearence changed");
            if (carAppearenceChanged != null)
                carAppearenceChanged();
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
        public static void ShieldDestroyed()
        {
            Debug.Log("Shield destroyed");
            if (shieldDestroyed != null)
                shieldDestroyed();
        }
        public static void Initialized()
        {
            Debug.Log("Game engine initialized");
            if (initialized != null)
                initialized();
        }
        public static void LevelGenerated()
        {
            Debug.Log("Level generated");
            if (levelGenerated != null)
                levelGenerated();
        }
        public static void Paused()
        {
            Debug.Log("Paused");
            if (paused != null)
                paused();
        }
        public static void Unpaused()
        {
            Debug.Log("Unpaused");
            if (unpaused != null)
                unpaused();
        }
        public static void NitroActivated()
        {
            Debug.Log("Nitro activated");
            if (nitroActivated != null)
                nitroActivated();
        }
        public static void TimeEventOccured(string EventName)
        {
            Debug.Log("Time event " + EventName + " occured at " + DateTime.Now.ToString());
            if (timeEventOccured != null)
                timeEventOccured(EventName);
        }
        public static void ZoneReached(GameObject zone)
        {
            Debug.Log(zone.name + " reached");
            if (zoneReached != null)
                zoneReached(zone);
        }
        public static void ZoneLeft(GameObject zone)
        {
            Debug.Log(zone.name + " left");
            if (zoneLeft != null)
                zoneLeft(zone);
        }
        public static void AdLoaded(PlacementType type)
        {
            Debug.Log(type + " placement loaded");
            if (adLoaded != null)
                adLoaded(type);
        }
        public static void AdNotReady(PlacementType type)
        {
            Debug.Log("Time to use " + type + " placement, but it is not ready");
            if (adNotReady != null)
                adNotReady(type);
        }
        public static void AdFinished(PlacementType type)
        {
            Debug.Log(type + " placement finished");
            if (adFinished != null)
                adFinished(type);
        }
        public static void AdSkipped(PlacementType type)
        {
            Debug.Log(type + " placement skipped");
            if (adSkipped != null)
                adSkipped(type);
        }
        public static void AdFailed(PlacementType type)
        {
            Debug.LogError(type + " placement failed");
            if (adFailed != null)
                adFailed(type);
        }
        public static void AdOpened(PlacementType type)
        {
            Debug.Log(type + " placement clicked");
            if (adOpened != null)
                adOpened(type);
        }
        public static void AdUserLeave(PlacementType type)
        {
            Debug.Log("User left, wathing advertisment " + type);
            if (adUserLeave != null)
                adUserLeave(type);
        }
        public static void GarageStateChanged(GarageCoinMakerType type)
        {
            Debug.Log("Coin maker " + type + " upgraded");
            if (onGarageStateChanged != null)
                onGarageStateChanged(type);
        }
        public static void CarUpgradeStateChanged(CarUpgradeType Type)
        {
            Debug.Log("Car " + Type + " upgraded");
            if (onCarUpgradeStateChanged != null)
                onCarUpgradeStateChanged(Type);
        }
    }
}
public enum GameSessionState { InProgress,Passed, Won, Lost }
public enum GarageCoinMakerType { MaterialsPedestal, MaterialsShelf, ToolsWall, Workbench }
public enum CarAppearenceState { Locked, Passed, Unlocked, Missing }
public enum CoinMakerStates { Blocked, NotPurchased, Normal, Unpacking, UnpackingFinished, MaxLevelReached }
public enum CarUpgradeState { Blocked, Normal, TopLevelReached}
public enum CarUpgradeType { Engine, Gearbox, Brakes}
public enum TVSetState { RecentlyWatched, VideoNotReady, ReadyToWatch}
public enum NitroState { Blocked, NotPurchased, Available, Empty}