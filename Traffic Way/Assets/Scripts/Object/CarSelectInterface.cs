using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectInterface : MonoBehaviour
{
    private static Texture2D navigationDotActive;
    private static Texture2D navigationDotInactive;
    private float navigationDotsDistance = 10f;
    private float navigationDotsSize = 5f;
    private static int tappedPanelNum;
    private static int _currentPage;

    private Transform _navigationPanel
    {
        get { return transform.GetChild(0).GetChild(0).GetChild(3); }
    }
    private Transform _turnLeftButton
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(0);
        }
    }
    private Transform _turnRightButton
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(1);
        }
    }
    private Transform _carAppearencesPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(1);
        }
    }
    private int _pagesAmt
    {
        get 
        {
            if (Engine.CarsAppearences.Count % 6 > 0)
                return Engine.CarsAppearences.Count / 6 + 1;
            else
                return Engine.CarsAppearences.Count / 6;
        }
    }

    public static void LoadResources()
    {
        navigationDotActive = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/NavDotActive");
        navigationDotInactive = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/NavDotInactive");
    }
    public static void UnlockAppearence()
    {
        Engine.meta.car.UnlockAppearence(_currentPage * 6 + tappedPanelNum);
    }

    public void SwipeRight()
    {
        _currentPage++;
        ShowPage();
    }
    public void SwipeLeft()
    {
        _currentPage--;
        ShowPage();
    }
    public void Open()
    {
        _currentPage = 0;
        ShowPage();
    }
    public void Refresh()
    {
        ShowPage();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        Engine.Events.adLoaded -= ActivateButtonAndUnsubscribe;
    }
    public void AppearenceTapped(int panelNum)
    {
        tappedPanelNum = panelNum;
        switch (Engine.GetAppearenceState(_currentPage * 6 + panelNum))
        {
            case CarAppearenceState.Unlocked:
                Engine.meta.car.SwitchCurrentAppearenceTo(_currentPage * 6 + panelNum);
                SwitchContent();
                break;
            case CarAppearenceState.Locked:
                Debug.Log("It is locked");
                break;
            case CarAppearenceState.Passed:
                Debug.Log("It is passed");
                transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                    Engine.CarsAppearences[_currentPage * 6 + panelNum],
                    new Rect(0f, 0f, Engine.CarsAppearences[_currentPage * 6 + panelNum].width, Engine.CarsAppearences[_currentPage * 6 + panelNum].height),
                    Vector2.one * 0.5f);
                transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                    Engine.CarsAppearences[_currentPage * 6 + panelNum],
                    new Rect(0f, 0f, Engine.CarsAppearences[_currentPage * 6 + panelNum].width, Engine.CarsAppearences[_currentPage * 6 + panelNum].height),
                    Vector2.one * 0.5f);
                if (AdMobController.isRewardedVideoReady)
                    transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().interactable = true;
                else
                    Engine.Events.adLoaded += ActivateButtonAndUnsubscribe;
                break;
            default:
                Debug.Log("Unknown appearence " + (_currentPage * 6 + panelNum).ToString() + " state " + Engine.GetAppearenceState(_currentPage * 6 + panelNum).ToString());
                break;
        }
    }

    private void ActivateButtonAndUnsubscribe(PlacementType type)
    {
        if (type == PlacementType.rewardedVideo)
            transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().interactable = true;
        Engine.Events.adLoaded -= ActivateButtonAndUnsubscribe;
    }
    private void ShowPage()
    {
        SwitchControls();
        SwitchContent();
    }
    private void SwitchControls()
    {
        //Clear all dots
        for (int i = _navigationPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(_navigationPanel.GetChild(i).gameObject);
        }

        //Compute dots position
        float _navDotsShift;
        if (_pagesAmt % 2 == 0)
            _navDotsShift = ((_pagesAmt - 1) / 2) * navigationDotsDistance + navigationDotsDistance / 2;
        else
            _navDotsShift = ((_pagesAmt - 1) / 2) * navigationDotsDistance;

        //Create dots
        RectTransform _dt;
        for (int i = 0; i < _pagesAmt; i++)
        {
            _dt = new GameObject().AddComponent<RectTransform>();
            _dt.SetParent(_navigationPanel,false);
            _dt.gameObject.AddComponent<CanvasRenderer>();
            _dt.gameObject.AddComponent<Image>();
            if (i == _currentPage)
                _dt.gameObject.GetComponent<Image>().sprite = Sprite.Create(navigationDotActive, new Rect(0f, 0f, navigationDotActive.width, navigationDotActive.height), Vector2.one * 0.5f, 100f);
            else
                _dt.gameObject.GetComponent<Image>().sprite = Sprite.Create(navigationDotInactive, new Rect(0f, 0f, navigationDotInactive.width, navigationDotInactive.height), Vector2.one * 0.5f, 100f);
            _dt.anchoredPosition = new Vector2(i * navigationDotsDistance - _navDotsShift, 0f);
            _dt.sizeDelta = Vector2.one * navigationDotsSize;
            _dt.anchorMax = Vector2.one * 0.5f;
            _dt.anchorMin = Vector2.one * 0.5f;
            _dt.pivot = Vector2.one * 0.5f;
        }

        //Manage controls
        if (_currentPage == 0)
        {
            _turnLeftButton.gameObject.SetActive(false);
            _turnRightButton.gameObject.SetActive(true);
        }
        else
            if (_currentPage == (_pagesAmt - 1))
            {
                _turnLeftButton.gameObject.SetActive(true);
                _turnRightButton.gameObject.SetActive(false);
            }
            else
            {
                _turnLeftButton.gameObject.SetActive(true);
                _turnRightButton.gameObject.SetActive(true);
            }
    }
    private void SwitchContent()
    {
        for (int i = 0; i < 6; i++)
        {
            _carAppearencesPanel.GetChild(i).GetComponent<Image>().color = Color.white;
            Debug.Log("Car " + (_currentPage * 6 + i) + " is " + Engine.GetAppearenceState(_currentPage * 6 + i).ToString());
            switch (Engine.GetAppearenceState(_currentPage * 6 + i))
            {
                case CarAppearenceState.Missing:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(false);
                    break;
                case CarAppearenceState.Locked:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarAppearencesShadows[_currentPage * 6 + i]
                        , new Rect(0f, 0f, Engine.CarAppearencesShadows[_currentPage * 6 + i].width, Engine.CarAppearencesShadows[_currentPage * 6 + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(1).GetComponent<Text>().text = Localization.GetLocal("LevelToOpenACar") + ComputeLevelForAppearence(_currentPage * 6 + i).ToString();
                    _carAppearencesPanel.GetChild(i).GetChild(2).gameObject.SetActive(true);
                    break;
                case CarAppearenceState.Unlocked:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarsAppearences[_currentPage * 6 + i]
                        , new Rect(0f, 0f, Engine.CarsAppearences[_currentPage * 6 + i].width, Engine.CarsAppearences[_currentPage * 6 + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    _carAppearencesPanel.GetChild(i).GetChild(2).gameObject.SetActive(false);
                    if (Engine.meta.car.currentAppearenceNum == _currentPage * 6 + i)
                        _carAppearencesPanel.GetChild(i).GetComponent<Image>().color = Color.yellow;
                    break;
                case CarAppearenceState.Passed:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarAppearencesShadows[_currentPage * 6 + i]
                        , new Rect(0f, 0f, Engine.CarAppearencesShadows[_currentPage * 6 + i].width, Engine.CarAppearencesShadows[_currentPage * 6 + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(1).GetComponent<Text>().text = Localization.GetLocal("WatchVideoToOpenACar");
                    _carAppearencesPanel.GetChild(i).GetChild(2).gameObject.SetActive(false);
                    break;
                default:
                    Debug.LogError("Unknown car appearence type: " + Engine.GetAppearenceState(_currentPage * 6 + i));
                    break;
            }
        }
    }
    private int ComputeLevelForAppearence(int appNum)
    {
        int appearencesToReach = appNum - Engine.meta.car.nextPassedAppearenceNum;
        int levelsPerAppearence = Mathf.CeilToInt(100f / Settings.levelCarProgress);
        int currentAppearenceLevels = Mathf.CeilToInt((100f - Engine.meta.car.nextAppearenceProgress) / Settings.levelCarProgress);
        return Engine.actualLevel + currentAppearenceLevels + levelsPerAppearence * appearencesToReach - 1;
    }
}