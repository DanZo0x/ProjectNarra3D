using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        SaveSystem.instance.LoadData();
    }

    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveSystem.instance.SaveData();
    }
}
