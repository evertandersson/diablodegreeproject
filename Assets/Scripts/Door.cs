using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game
{
    public class Door : MonoBehaviour, Interactable
    {
        [SerializeField] private ItemSO key;

        public enum State
        {
            Locked,
            Closed,
            Open
        }

        public State state;

        public void Trigger()
        {
            switch (state)
            {
                case State.Locked:
                    CheckIfPlayerHasKey();
                    break;
                case State.Closed:
                    state = State.Open;
                    StopAllCoroutines();
                    StartCoroutine(PlayDoorAnimation());
                    break;
                case State.Open:
                    state = State.Closed;
                    StopAllCoroutines();
                    StartCoroutine(PlayDoorAnimation());
                    break;
            }
        }

        private void CheckIfPlayerHasKey()
        {
            foreach (InventorySlot slot in PlayerManager.Instance.inventory.inventory)
            {
                if (slot.item != null)
                {
                    if (slot.item.name == key.name)
                    {
                        slot.RemoveItem();
                        state = State.Closed;
                        break;
                    }
                }
            }
        }

        private IEnumerator PlayDoorAnimation()
        {
            Quaternion targetRotation;

            if (state == State.Open)
            {
                // Rotate to open position (-90 degrees around the Y-axis)
                targetRotation = Quaternion.Euler(0, -90, 0);
            }
            else if (state == State.Closed)
            {
                // Rotate back to closed position (0 degrees around the Y-axis)
                targetRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                yield break; // Exit if the state is not Open or Closed
            }

            // Smoothly rotate over time
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3);
                yield return null; // Wait until the next frame
            }

            // Snap to the exact target rotation to avoid small inaccuracies
            transform.rotation = targetRotation;
        }

    }

}