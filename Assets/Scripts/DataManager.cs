using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static public DataManager Instance;


    Dictionary<string, bool> boolPropertyDict = new Dictionary<string, bool>()
    {
        { "GoldenBook", false },
        { "Rose", false },
        { "Pens�e", false },
        { "Dahlia", false },
        { "LysIridescent", false },
        { "Lys", false },
        { "BrocheOrn�e", false },
        { "Marked", false }
    };

    public Dictionary<string, bool> BoolPropertyDict { get => boolPropertyDict; set => boolPropertyDict = value; }

    public List<DateData> dates = new List<DateData>();

    [Serializable]
    public class PhoneNumberData
    {
        public string dateName;
        public string phoneNumber;
        [HideInInspector] public int iterationDate;

    }

    public List<PhoneNumberData> phoneNumbers;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }



    public DateData FindFromName(string name)
    {
        foreach(DateData date in dates)
        {
            if(date.name == name)
            {
                return date;
            }
        }
        return null;
    }

   
}
