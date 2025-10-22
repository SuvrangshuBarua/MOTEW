using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Story System/Dialogue Data", order = 51)]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        [TextArea(3, 10)]
        public string text;
    }
    public List<Line> lines = new List<Line>();
}