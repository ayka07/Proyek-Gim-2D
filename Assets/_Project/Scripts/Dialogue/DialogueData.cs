using System;
using UnityEngine;

[Serializable]
public struct DialogueLine
{
    public string speaker;
    [TextArea(2, 5)] public string text;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "Monster/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}
