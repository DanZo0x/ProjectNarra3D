using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static public DataManager Instance;


    Dictionary<string, bool> boolPropertyDict = new Dictionary<string, bool>()
    {
        { "hasFeather", false },
    };

    public Dictionary<string, bool> BoolPropertyDict { get => boolPropertyDict; set => boolPropertyDict = value; }

    public List<DateData> dates = new List<DateData>();

    public enum Speaker
    {
        Hecat,
        Muller,
        Lilith,
        Noharnaak
    };
    public Speaker speaker;
    
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
