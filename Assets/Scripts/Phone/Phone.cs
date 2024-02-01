using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Subtegral.DialogueSystem.Runtime;
using UnityEngine.Experimental.GlobalIllumination;




public class Phone : MonoBehaviour
{
    [SerializeField] Animation telephone;
    [SerializeField] Transform valuePaper;
    [SerializeField] Color colorValue;
    [SerializeField] Light _light;
    bool canInteract;
    Transform touches;
    string _charHolder = "";
    int charCount = 0;
    [SerializeField] List<Sprite> spritesLetters = new List<Sprite>();
    

    private void Awake()
    {
        touches = telephone.transform.GetChild(3);
        canInteract = true;
    }
    public void N1()
    {
        if (canInteract)
        {
            _charHolder += "1";
            touches.GetChild(3).GetComponent<Animator>().SetTrigger("PressUL");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[0];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;
            charCount++;
            CheckTriesNumber();
        }
    }
    public void N2()
    {
        if (canInteract)
        {
            _charHolder += "2";

            touches.GetChild(2).GetComponent<Animator>().SetTrigger("PressUR");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[1];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;
            charCount++;
            CheckTriesNumber();
        }
            
        
    }
    public void N3()
    {
        if (canInteract)
        {
            _charHolder += "3";
            touches.GetChild(5).GetComponent<Animator>().SetTrigger("PressML");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[2];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;
            charCount++;
            CheckTriesNumber();
        }
        
        
    }
    public void N4()
    {
        if (canInteract)
        {
            _charHolder += "4";
            touches.GetChild(4).GetComponent<Animator>().SetTrigger("PressMR");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[3];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;
            charCount++;
            CheckTriesNumber();
        }
        
        
    }
    public void N5()
    {
        if (canInteract)
        {
            _charHolder += "5";
            touches.GetChild(1).GetComponent<Animator>().SetTrigger("PressDL");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[4];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;
            charCount++;
            CheckTriesNumber();
        }
        
    }
    public void N6()
    {
        if (canInteract)
        {
            _charHolder += "6";
            touches.GetChild(0).GetComponent<Animator>().SetTrigger("PressDR");
            valuePaper.GetChild(charCount).GetComponent<Image>().sprite = spritesLetters[5];
            valuePaper.GetChild(charCount).GetComponent<Image>().color = colorValue;

            charCount++;
            CheckTriesNumber();
        }
        
       
    }
    
    public void Call()
    {
        if (_charHolder == "666")
        {
            Debug.Log("Vous êtes bien sur le répondeur de satan ne laissez pas de message après le cri des âmes damnées");
        }
        else if(_charHolder== "01123581321")
        {
            Debug.Log("BELPHEGOR");
        }
        else if (_charHolder == "0761964399")
        {
            Application.OpenURL("tel://0761964399");
        }
        else if (_charHolder == "0612047223")
        {
            Application.OpenURL("tel://0612047223");
        }

        else
        {
            Debug.Log("Le numéro composé n'a pas enocre été attribué");
        }
    }

    void CheckTriesNumber() 
    {
        if(charCount == 6)
        {
            canInteract = false;
            foreach(var numbers in DataManager.Instance.phoneNumbers)
            {
                if(numbers.phoneNumber == _charHolder)
                {
                    Debug.Log(numbers.dateName); 
                    string date = "";
                    if (numbers.iteration == 0)
                    {
                        date = numbers.dateName + "Date1";
                    }
                    else if (numbers.iteration == 1)
                    {
                        date = numbers.dateName + "Date2";
                    }
                    DialogConfig.Instance.transform.GetComponent<DialogueParser>().NewDialogue(date);
                    break;
                }
            }
            StartCoroutine(WrongValue());
        }
    }

    IEnumerator WrongValue()
    {
        _light.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        Reset();
        
    }

    private void Reset()
    {
        _light.color = Color.white;
        foreach (Transform child in valuePaper)
        {
            child.GetComponent<Image>().sprite = null;
            child.GetComponent<Image>().color = Color.white;
        }
        _charHolder = "";
        charCount = 0;
        StartCoroutine(transform.parent.parent.GetComponent<ShowPhone>().DezoomPhoneCoroutine());
        transform.parent.parent.GetComponent<ShowPhone>().OutlineAllButtons(false);
        canInteract = true;
        transform.parent.parent.GetComponent<ShowPhone>().zoomButton.gameObject.SetActive(true);
    }
}
