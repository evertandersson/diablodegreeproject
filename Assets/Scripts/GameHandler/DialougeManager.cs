using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialougeManager : EventHandler.GameEventBehaviour
{
    public static DialougeManager Instance;

    [SerializeField] private GameObject dialougeBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialougeText;
    public NPC currentNPC;

    bool isDone = false;

    private Queue<string> sentences;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialougeBox.SetActive(false);
        sentences = new Queue<string>();        
    }

    public void StartDialouge(Dialouge dialouge, NPC npc)
    {
        isDone = false;

        currentNPC = npc;

        EventHandler.Main.PushEvent(this);

        dialougeBox.SetActive(true);

        nameText.text = dialouge.name;

        sentences.Clear();

        foreach(string sentence in dialouge.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            isDone = true;
            return;
        }

        string sentence = sentences.Dequeue();
        dialougeText.text = sentence;

    }

    public override void OnEnd()
    {
        base.OnEnd();
        currentNPC = null;
        dialougeBox.SetActive(false);
    }

    public override bool IsDone()
    {
        return isDone;
    }
}
