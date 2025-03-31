using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Choice", menuName = "Dialog System/Dialog Choice")]

public class DialogChoiceSO : ScriptableObject
{
    // Start is called before the first frame update
    public string text;
    public int nextId;

}
