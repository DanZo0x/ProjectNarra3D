using Subtegral.DialogueSystem.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Date Data", menuName = "Database/Date", order = 1)]
public class DateData : ScriptableObject
{
    public DialogueContainer dialogue;

    [SerializeField]
    public TextAsset table;
    public Sprite background;
    public Sprite buttonSprite;
}
