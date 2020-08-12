using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectInterface : MonoBehaviour
{
    private static Texture2D navigationDotActive;
    private static Texture2D navigationDotInactive;
    private static Texture2D regularCarBackground;
    private static Texture2D selectedCarBackground;
    private float navigationDotsDistance = 12f;
    private float navigationDotsSize = 6f;
    private static int tappedPanelNum;
    private static int _currentPage;
    private const int _carsOnPage = 4;

    private Transform _navigationPanel
    {
        get { return transform.GetChild(1).GetChild(0).GetChild(3); }
    }
    private Transform _turnLeftButton
    {
        get
        {
            return transform.GetChild(1).GetChild(0).GetChild(0);
        }
    }
    private Transform _turnRightButton
    {
        get
        {
            return transform.GetChild(1).GetChild(0).GetChild(1);
        }
    }
    private Transform _carAppearencesPanel
    {
        get
        {
            return transform.GetChild(1).GetChild(1);
        }
    }
    //private Transform _carUnlockConfirmationPanel
    //{
    //    get
    //    {
    //        return transform.GetChild(2);
    //    }
    //}
    private Transform _carUnlockedPanel
    {
        get
        {
            return transform.GetChild(2);
        }
    }
    private Transform _carSelectPanel
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private int _pagesAmt
    {
        get 
        {
            if (Engine.CarsAppearencesAngled.Count % _carsOnPage > 0)
                return Engine.CarsAppearencesAngled.Count / _carsOnPage + 1;
            else
                return Engine.CarsAppearencesAngled.Count / _carsOnPage;
        }
    }

    public static void LoadResources()
    {
        navigationDotActive = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/ChooseCar/choose_dot_1");
        navigationDotInactive = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/ChooseCar/choose_dot_2");
        regularCarBackground = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/ChooseCar/unpick_car_bg");
        selectedCarBackground = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/ChooseCar/pick_car_bg");
    }
    public static void UnlockAppearence()
    {
        Engine.meta.car.UnlockAppearence(_currentPage * _carsOnPage + tappedPanelNum);
    }

    public void SwipeRight()
    {
        _currentPage++;
        ShowPage();
        Debug.Log("Swiped right");
    }
    public void SwipeLeft()
    {
        _currentPage--;
        ShowPage();
        Debug.Log("Swiped left");
    }
    public void Open()
    {
        _carSelectPanel.gameObject.SetActive(true);
        //_carUnlockConfirmationPanel.gameObject.SetActive(false);
        _carUnlockedPanel.gameObject.SetActive(false);
        _currentPage = 0;
        ShowPage();
    }
    public void Refresh()
    {
        _carSelectPanel.gameObject.SetActive(true);
        //_carUnlockConfirmationPanel.gameObject.SetActive(false);
        _carUnlockedPanel.gameObject.SetActive(false);
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
        switch (Engine.GetAppearenceState(_currentPage * _carsOnPage + panelNum))
        {
            case CarAppearenceState.Unlocked:
                Engine.meta.car.SwitchCurrentAppearenceTo(_currentPage * _carsOnPage + panelNum);
                SwitchContent();
                break;
            case CarAppearenceState.Locked:
                Debug.Log("It is locked");
                break;
            case CarAppearenceState.Passed:
                Debug.Log("It is passed");
                if (!AdMobController.isRewardedVideoReady)
                    return;
                transform.parent.parent.gameObject.GetComponent<UserInteraction>().ShowCarAds();
                //_carUnlockConfirmationPanel.gameObject.SetActive(true);
                //_carUnlockConfirmationPanel.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                //    Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum],
                //    new Rect(0f, 0f, Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum].width, Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum].height),
                //    Vector2.one * 0.5f);
                _carUnlockedPanel.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                    Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum],
                    new Rect(0f, 0f, Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum].width, Engine.CarsAppearences[_currentPage * _carsOnPage + panelNum].height),
                    Vector2.one * 0.5f);
                //if (AdMobController.isRewardedVideoReady)
                //    _carUnlockConfirmationPanel.GetChild(1).GetComponent<Button>().interactable = true;
                //else
                //    Engine.Events.adLoaded += ActivateButtonAndUnsubscribe;
                //_carSelectPanel.gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Unknown appearence " + (_currentPage * _carsOnPage + panelNum).ToString() + " state " + Engine.GetAppearenceState(_currentPage * _carsOnPage + panelNum).ToString());
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
        for (int i = 0; i < _carsOnPage; i++)
        {
            _carAppearencesPanel.GetChild(i).GetComponent<Image>().color = Color.white;
            Debug.Log("Car " + (_currentPage * _carsOnPage + i) + " is " + Engine.GetAppearenceState(_currentPage * _carsOnPage + i).ToString());
            switch (Engine.GetAppearenceState(_currentPage * _carsOnPage + i))
            {
                case CarAppearenceState.Missing:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(false);
                    break;
                case CarAppearenceState.Locked:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i]
                        , new Rect(0f, 0f, Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i].width, Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    _carAppearencesPanel.GetChild(i).GetComponent<Image>().sprite = Sprite.Create(
                        regularCarBackground
                        , new Rect(0f, 0f, regularCarBackground.width, regularCarBackground.height)
                        , Vector2.one * 0.5f);
                    break;
                case CarAppearenceState.Unlocked:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarsAppearencesAngled[_currentPage * _carsOnPage + i]
                        , new Rect(0f, 0f, Engine.CarsAppearencesAngled[_currentPage * _carsOnPage + i].width, Engine.CarsAppearencesAngled[_currentPage * _carsOnPage + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    if (Engine.meta.car.currentAppearenceNum == _currentPage * _carsOnPage + i)
                        _carAppearencesPanel.GetChild(i).GetComponent<Image>().sprite = Sprite.Create(
                            selectedCarBackground
                            , new Rect(0f, 0f, selectedCarBackground.width, selectedCarBackground.height)
                            , Vector2.one * 0.5f);
                    else
                        _carAppearencesPanel.GetChild(i).GetComponent<Image>().sprite = Sprite.Create(
                            regularCarBackground
                            , new Rect(0f, 0f, regularCarBackground.width, regularCarBackground.height)
                            , Vector2.one * 0.5f);
                    break;
                case CarAppearenceState.Passed:
                    _carAppearencesPanel.GetChild(i).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(
                        Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i]
                        , new Rect(0f, 0f, Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i].width, Engine.CarAppearencesShadowsAngled[_currentPage * _carsOnPage + i].height)
                        , Vector2.one * 0.5f);
                    _carAppearencesPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    _carAppearencesPanel.GetChild(i).GetComponent<Image>().sprite = Sprite.Create(
                        regularCarBackground
                        , new Rect(0f, 0f, regularCarBackground.width, regularCarBackground.height)
                        , Vector2.one * 0.5f);
                    break;
                default:
                    Debug.LogError("Unknown car appearence type: " + Engine.GetAppearenceState(_currentPage * _carsOnPage + i));
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