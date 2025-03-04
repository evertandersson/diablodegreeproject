using Game;
using UnityEngine;

public class DoorTriggerNPC : MonoBehaviour
{
    [SerializeField] private DoorNPC parentDoor;

    private void Awake()
    {
        parentDoor = GetComponentInParent<DoorNPC>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentDoor.state == Door.State.Open || parentDoor.state == Door.State.Closed)
            return;

        if (other.TryGetComponent<WomanNPC>(out WomanNPC womanNpc))
        {
            womanNpc.SetTargetDoor(parentDoor);
        }
    }
}
