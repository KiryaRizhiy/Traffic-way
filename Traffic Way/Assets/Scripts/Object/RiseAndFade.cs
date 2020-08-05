using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RiseAndFade : MonoBehaviour
{
    public float moveSpeed;
    public float fullFadeTime;

    private float _passedTime;
    private Color _initialImgColor;
    private Color _targetImgColor;
    private Color _initialTextColor;
    private Color _targetTextColor;
    private bool _hasChildTextObj;
    private bool _imageComponent;

    void Start()
    {
        StartCoroutine(PlayRiseAndFade());
        _passedTime = 0f; // Обнуляем время, прошедшее с момента старта
        if (GetComponent<Image>() == null)
        {
            _imageComponent = false;
            _initialImgColor = GetComponent<RawImage>().color; //Сохраняем изначальный цвет изображения
        }
        else
        {
            _imageComponent = true;
            _initialImgColor = GetComponent<Image>().color; //Сохраняем изначальный цвет изображения
        }
        _targetImgColor = new Color(_initialImgColor.r, _initialImgColor.g, _initialImgColor.b, 0f);
        if (transform.childCount > 0) //Проверяем, имеет ли наш объект дочерние объекты
            if (transform.GetChild(0).GetComponent<Text>() != null) //Проверяем, есть ли текст на первом дочернем объекте
            {
                _hasChildTextObj = true; //Сохраняем, что есть дочерний текстовый объект (для экономии дальнейших вычислений)
                _initialTextColor = transform.GetChild(0).GetComponent<Text>().color; //Сохраняем изначальный цвет текста
                _targetTextColor = new Color(_initialTextColor.r, _initialTextColor.g, _initialTextColor.b, 0f);
            }
            else
                _hasChildTextObj = false;
    }

    private IEnumerator PlayRiseAndFade()
    {
        while (_passedTime < fullFadeTime)//Описываем условие выхода из цикла
        {
            _passedTime += Time.deltaTime; //Учитываем прошедшее время
            GetComponent<RectTransform>().Translate(Vector3.up * moveSpeed * Time.deltaTime);//Сдвиг позиции со временем
            if (_hasChildTextObj)
                transform.GetChild(0).GetComponent<Text>().color = Color.Lerp(
                                       _initialTextColor,
                                       _targetTextColor,
                                       _passedTime / fullFadeTime); //Устанавливаем тексту интерполированный цвет
            if (_imageComponent)
                GetComponent<Image>().color = Color.Lerp(
                                           _initialImgColor,
                                           _targetImgColor,
                                           _passedTime / fullFadeTime); //Устанавливаем картинке интерполированный цвет
            else
                GetComponent<RawImage>().color = Color.Lerp(
                                           _initialImgColor,
                                           _targetImgColor,
                                           _passedTime / fullFadeTime); //Устанавливаем картинке интерполированный цвет

            yield return new WaitForEndOfFrame();
        }
        //Повторяем логику корутины в граничном состоянии, когда _passedTime = fullFadeTime
        if (_hasChildTextObj)
            transform.GetChild(0).GetComponent<Text>().color = Color.Lerp(_initialTextColor, _targetTextColor, 1f);
        if (_imageComponent)
            GetComponent<Image>().color = Color.Lerp(_initialImgColor, _targetImgColor, 1f);
        else
            GetComponent<RawImage>().color = Color.Lerp(_initialImgColor, _targetImgColor, 1f);
        Destroy(gameObject, 1f); //Уничтожаем полностью прозрачный объект через секунду
    }
}