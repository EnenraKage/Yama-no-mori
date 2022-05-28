using UnityEngine;
using UnityEngine.UI;

//all of this is to create scriptable objects, use this as reference for the future!!!
[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string text;
    public Conversation conversation;
}

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    [TextArea(2, 5)]
    public string text;
    public Choice[] choices;
}
