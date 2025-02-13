using System.Collections.Generic;
using UnityEngine;

public class DialougeManager : MonoBehaviour
{
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();        
    }

}
