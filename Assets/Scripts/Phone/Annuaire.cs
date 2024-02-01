using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Annuaire : MonoBehaviour
{
    
    Vector2 _originScale;
    Vector2 _originPosition;
    [SerializeField] Vector3 _goalPos;
    [SerializeField] Vector3 _goalScale;
    [SerializeField] float _time;
    RectTransform annuaire;
    Transform spriteAnnuaire;


    private void Awake()
    {
        annuaire = transform.Find("AnnuaireContent").GetComponent<RectTransform>();
        spriteAnnuaire = transform.Find("Sprite");
        _originScale = annuaire.localScale;
        _originPosition = annuaire.anchoredPosition;
        annuaire.gameObject.SetActive(false);
    }

    public IEnumerator OpenAnnuaireCoroutine()
    {
        annuaire.gameObject.SetActive(true);
        spriteAnnuaire.gameObject.SetActive(false);
        float timer = 0;
        while (timer < _time)
        {
            float percentage = timer / _time;
            annuaire.anchorMin = Vector2.Lerp(annuaire.anchorMin, new Vector2(0.5f, 0.5f), percentage);
            annuaire.anchorMax = Vector2.Lerp(annuaire.anchorMax, new Vector2(0.5f, 0.5f), percentage);
            annuaire.anchoredPosition = Vector2.Lerp(_originPosition, new Vector2(0, 0), percentage);
            annuaire.localScale = Vector2.Lerp(_originScale, _goalScale, percentage);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator CloseAnnuaireCoroutine()
    {
        annuaire.gameObject.SetActive(false);
        spriteAnnuaire.gameObject.SetActive(true);
        Vector2 vectorPos = annuaire.anchoredPosition;
        Vector2 scale = annuaire.localScale;
        float timer = 0;
        while (timer < _time)
        {
            float percentage = timer / _time;
            annuaire.anchorMin = Vector2.Lerp(annuaire.anchorMin, new Vector2(0.5f, 0), percentage);
            annuaire.anchorMax = Vector2.Lerp(annuaire.anchorMax, new Vector2(0.5f, 0), percentage);
            annuaire.anchoredPosition = Vector2.Lerp(vectorPos, _originPosition, percentage);
            annuaire.localScale = Vector2.Lerp(scale, _originScale, percentage);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void OpenAnnuaire(bool value)
    {
        if(value)
        {
            StartCoroutine(OpenAnnuaireCoroutine());
        }
        else
        {
            StartCoroutine(CloseAnnuaireCoroutine());
        }
    }
}
