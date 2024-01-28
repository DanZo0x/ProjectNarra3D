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

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}
