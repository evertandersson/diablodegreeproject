using UnityEngine;

[System.Serializable]
public class Dialouge
{
    public string name;

    public Expression[] expressions;
}

[System.Serializable]
public class Expression
{
    [TextArea(3, 10)]
    public string sentence;

    public string animation;
}
