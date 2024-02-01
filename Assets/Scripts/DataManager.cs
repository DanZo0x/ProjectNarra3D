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
        { "Pensée", false },
        { "Dahlia", false },
        { "LysIridescent", false },
        { "Lys", false },
        { "BrocheOrnée", false },
        { "Marked", false },

        { "AffinityMummy > 1", false},
        { "AffinityMummy > -2", false},
        { "AffinityMummy < -1", false},
        {"AffinityBooksellers >= 8", false},

        { "SawAngel", false},
        { "SawMummy", false},
        { "SawMuller", false},
        { "SawHecat", false},
    };
    int peopleMet = 0;
    public Dictionary<string, bool> BoolPropertyDict { get => boolPropertyDict; set => boolPropertyDict = value; }

    public List<DateData> dates = new List<DateData>();

    private void Update()
    {
        SetVariables();
    }
    private void SetVariables()
    {
        #region Mummy
        var mummy = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(y => y.label == "Noharnaak");
        if (mummy.affection > 1)
        {
            boolPropertyDict["AffinityMummy > 1"] = true;
            boolPropertyDict["AffinityMummy > -2"] = true;
            boolPropertyDict["AffinityMummy < -1"] = false;
        }
        else if(mummy.affection > -2)
        {
            boolPropertyDict["AffinityMummy > 1"] = false;
            boolPropertyDict["AffinityMummy > -2"] = true;
            boolPropertyDict["AffinityMummy < -1"] = false;
        }
        else if( mummy.affection < -1)
        {
            boolPropertyDict["AffinityMummy > 1"] = false;
            boolPropertyDict["AffinityMummy > -2"] = false;
            boolPropertyDict["AffinityMummy < -1"] = true;
        }
        #endregion
        #region Hecat
        var hecat = DialogConfig.Instance.speakerDatabases[0].speakerDatas.Find(y => y.label == "Hecat");
        if (hecat.affection >= 8)
        {
            boolPropertyDict["AffinityBooksellers >= 8"] = true;
            
        }
        else 
        {
            boolPropertyDict["AffinityBooksellers >= 8"] = false;
        }

        #endregion
        if (peopleMet > 1)
        {
            boolPropertyDict["SawCharacters"] = true;
        }

    }

    public void MeetSomeone(string label)
    {
        switch (label)
        {
            case "Hecat":
                if(boolPropertyDict["SawHecat"] == false)
                {
                    peopleMet++;
                    boolPropertyDict["SawHecat"] = true;
                    
                }
                
                break;
            case "Noharnaak":
                if (boolPropertyDict["SawMummy"] == false)
                {
                    peopleMet++;
                    boolPropertyDict["SawMummy"] = true;

                }
                break;
            case "Lilith":
                if (boolPropertyDict["SawAngel"] == false)
                {
                    peopleMet++;
                    boolPropertyDict["SawAngel"] = true;

                }
                break;
            case "Muller":
                if (boolPropertyDict["SawMuller"] == false)
                {
                    peopleMet++;
                    boolPropertyDict["SawMuller"] = true;

                }
                break;
        }
    }

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
