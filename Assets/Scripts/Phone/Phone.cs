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
    [SerializeField] Transform telephone;
    Transform telephoneSpeaker;
    [SerializeField] Transform valuePaper;
    [SerializeField] Color colorValue;
    [SerializeField] Light _light;
    [SerializeField] Transform textAsk;
    
    bool canInteract;
    bool canPickUp;
    Transform touches;
    string _charHolder = "";
    int charCount = 0;
    string date = "";
    [SerializeField] List<Sprite> spritesLetters = new List<Sprite>();
    

    private void Awake()
    {
        touches = telephone.transform.GetChild(3);
        telephoneSpeaker = telephone.transform.GetChild(1);
        canInteract = true;
        canPickUp = false;
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
            float valueScale = 1.12f;
            canInteract = false;
            canPickUp = true;
            telephoneSpeaker.GetComponent<MeshRenderer>().materials[3].SetFloat("_Scale", valueScale);
        }
    }

    public void PickUpPhone()
    {
        if (canPickUp)
        {
            
            
            foreach (var numbers in DataManager.Instance.phoneNumbers)
            {
                if (numbers.phoneNumber == _charHolder)
                {
                   
                    Debug.Log(numbers.dateName);
                    if (numbers.iterationDate == 0)
                    {
                        date = numbers.dateName + "Date1";
                    }
                    else if (numbers.iterationDate == 1)
                    {
                        if(numbers.dateName == "Florist")
                        {
                            StartCoroutine(WrongValue());
                            return;
                        }

                        date = numbers.dateName + "Date2";
                    }

                    StartCoroutine(PickUpAnimation());
                    textAsk.GetComponent<TextMeshProUGUI>().text = $"Allez en date avec {numbers.dateName} ?";
                    return;

                }
            }
            StartCoroutine(WrongValue());
        }
    }


    public IEnumerator PickUpAnimation()
    {
        telephoneSpeaker.GetComponent<Animator>().SetTrigger("PickUp");
        StartCoroutine(transform.parent.parent.GetComponent<ShowPhone>().ZoomPhoneCoroutine(new Vector3(2.457f, 3.579f, 0.331f), new Quaternion(0.227508172f, -0.17129159f, 0.00806896575f, 0.95855844f)));
        yield return new WaitForSeconds(1.5f);
        textAsk.parent.gameObject.SetActive(true);
        
    }

    public void Accept()
    {
        DialogConfig.Instance.transform.GetComponent<DialogueParser>().NewDialogue(date);
    }

    public void Refuse()
    {
        ResetPhone();
    }
    

    IEnumerator WrongValue()
    {
        _light.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        ResetPhone();
        
    }

    public void ResetPhone()
    {
        _light.color = Color.white;
        foreach (Transform child in valuePaper)
        {
            child.GetComponent<Image>().sprite = null;
            child.GetComponent<Image>().color = Color.white;
        }
        
        _charHolder = "";
        charCount = 0;
        textAsk.parent.gameObject.SetActive(false);
        
        StartCoroutine(transform.parent.parent.GetComponent<ShowPhone>().DezoomPhoneCoroutine());
        transform.parent.parent.GetComponent<ShowPhone>().OutlineAllButtons(false);
        canInteract = true;
        transform.parent.parent.GetComponent<ShowPhone>().zoomButton.gameObject.SetActive(true);
        
        

    }
}
