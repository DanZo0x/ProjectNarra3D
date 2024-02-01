using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    [Header("References")]
    [SerializeField] private Slider _volumeSlider;

    [SerializeField] private TMP_Text _languageText;
    [SerializeField] private string _language;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetString("Language"));
        Debug.Log(PlayerPrefs.GetFloat("Volume"));
    }

    public void SwitchLanguageData()
    {
        if (PlayerPrefs.GetString("Language") == "EN")
        {
            PlayerPrefs.SetString("Language", "FR");
        }
        else
        {
            PlayerPrefs.SetString("Language", "EN");
        }
    }

    public void SetLanguageText()
    {
        if (PlayerPrefs.GetString("Language") == "EN")
        {
            _languageText.text = "LANGUAGE: ENGLISH";
        }
        else if (PlayerPrefs.GetString("Language") == "FR")
        {
            _languageText.text = "LANGAGE: FRANCAIS";
        }
    }

    public void SaveVolumeData()
    {
        PlayerPrefs.SetFloat("Volume", _volumeSlider.value);
        Debug.Log(PlayerPrefs.GetFloat("Volume"));
    }

    public void SaveLanguageData()
    {
        if (_languageText.text == "FRENCH")
        {
            PlayerPrefs.SetString("Language", "FR");
        }
        else if (_languageText.text == "ENGLISH")
        {
            PlayerPrefs.SetString("Language", "EN");
        }
    }

    public void SaveData()
    {
        SaveVolumeData();
        SaveLanguageData();
    }

    public void LoadData()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        _language = PlayerPrefs.GetString("Language");

        SetLanguageText();
    }

    public void DeleteData()
    {
        //KEYS DES SAVE DE PROGRESSION A METTRE
        //PlayerPrefs.DeleteKey("");
    }
}