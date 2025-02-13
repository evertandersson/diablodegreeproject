using Game;
using UnityEngine;

public class WomanNPC : NPC, Interactable
{
    public Dialouge dialouge;
    private Quaternion originalRotation;

    private float rotateSpeed = 2f;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    public Vector3 GetCenterPoint()
    {
        return transform.position;
    }

    public void Trigger()
    {
        DialougeManager.Instance.StartDialouge(dialouge, this);
    }

    private void Update()
    {
        if (DialougeManager.Instance.currentNPC == this)
        {
            Vector3 direction = (PlayerManager.Instance.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotateSpeed * Time.deltaTime);
        }
    }

}
