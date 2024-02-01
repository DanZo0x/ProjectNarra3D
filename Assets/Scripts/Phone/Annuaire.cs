using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Annuaire : MonoBehaviour
{
    bool _isOpenning = false;
    bool _isClosing = false;
    Vector2 _currentScale;
    RectTransform _rect;
    float _timer=0;
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_isOpenning)
        {
           _rect.anchorMin= Vector2.Lerp(_rect.anchorMin, new Vector2(0.5f, 0.5f), _timer/3);
           _rect.anchorMax= Vector2.Lerp(_rect.anchorMax, new Vector2(0.5f, 0.5f), _timer/3);
           _rect.anchoredPosition= Vector2.Lerp(_rect.anchoredPosition, new Vector2(0, 0), _timer/3);
            _rect.localScale = Vector2.Lerp(_rect.localScale, _currentScale * 2, _timer / 3);
            _timer += Time.deltaTime;
        }
        if (_isClosing)
        {
            _rect.anchorMin = Vector2.Lerp(_rect.anchorMin, new Vector2(0.5f, 0), _timer / 3);
            _rect.anchorMax = Vector2.Lerp(_rect.anchorMax, new Vector2(0.5f,0), _timer / 3);
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, new Vector2(0, 200), _timer / 3);
            _rect.localScale = Vector2.Lerp(_rect.localScale, _currentScale / 2, _timer / 3);
            _timer += Time.deltaTime;
        }
        if (_timer > 3)
        {
            _isClosing = false;
            _isOpenning = false;
            _timer = 0;
        }
    }
    public void Open()
    {
        _currentScale = _rect.localScale;
        _isOpenning = true;
        _isClosing = false;
        _timer = 0;
    }
    public void Close()
    {
        _currentScale = _rect.localScale;
        _isOpenning = false;
        _isClosing = true;
        _timer = 0;
    }
}
