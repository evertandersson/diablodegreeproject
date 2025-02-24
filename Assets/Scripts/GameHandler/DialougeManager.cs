using Game;
using System;
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

    public static event Action startDialouge;
    public static event Action endDialouge;

    bool isDone = false;

    private Queue<Expression> expressions;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialougeBox.SetActive(false);
        expressions = new Queue<Expression>();        
    }

    public void StartDialouge(Dialouge dialouge, NPC npc)
    {
        isDone = false;

        currentNPC = npc;

        EventHandler.Main.PushEvent(this);
        startDialouge?.Invoke();


        dialougeBox.SetActive(true);
        
        dialougeBox.transform.SetAsLastSibling();

        nameText.text = dialouge.name;

        expressions.Clear();

        foreach(Expression expression in dialouge.expressions)
        {
            expressions.Enqueue(expression);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (expressions.Count == 0)
        {
            isDone = true;
            return;
        }

        Expression expression = expressions.Dequeue();
        dialougeText.text = expression.sentence;
        
        if (expression.animation != "")
            currentNPC.animator.CrossFade(expression.animation, 0.2f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (PlayerManager.Instance)
        {
            PlayerManager.Instance.OnUpdate();   
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        currentNPC = null;
        endDialouge?.Invoke();
        dialougeBox.SetActive(false);
    }

    public override bool IsDone()
    {
        return isDone;
    }
}
