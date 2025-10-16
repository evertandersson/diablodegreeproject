using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game
{
    public class Door : Loadable, Interactable
    {
        [SerializeField] private ItemSO key;
        Renderer doorRenderer;

        Quaternion originalRotation;
        Quaternion openRotation;

        public enum State
        {
            Locked,
            Closed,
            Open
        }

        public State state;

        protected override void Awake()
        {
            base.Awake();
            doorRenderer = GetComponent<Renderer>();

            originalRotation = transform.rotation;
            openRotation = Quaternion.Euler(0, originalRotation.eulerAngles.y - 90f, 0);
        }

        protected override void Load()
        {
            if (SaveManager.Instance.doorsOpenedList.serializableList.Exists(door => door.name == id))
            {
                state = State.Closed;
                Trigger();
            }
        }

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
            foreach (InventorySlot slot in PlayerManager.Instance.inventory.inventorySlots)
            {
                if (slot.item != null && key != null)
                {
                    if (slot.item.name == key.name)
                    {
                        slot.RemoveItem();
                        state = State.Closed;

                        Vector3 offset = new Vector3(0, 1, -0.5f);
                        Vector3 textSpawnPosition = GetCenterPoint() + offset;

                        PopupText text = ObjectPooling.Instance.SpawnFromPool("PopupText", textSpawnPosition, Quaternion.identity).GetComponent<PopupText>();
                        text.message = "Unlocked door";
                        text.StartCoroutine("Trigger");
                        SoundManager.PlaySound(SoundType.DOOR);

                        SaveManager.Instance.AddObjectToList(id, SaveManager.Instance.doorsOpenedList);

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
                targetRotation = openRotation;
            }
            else if (state == State.Closed)
            {
                // Rotate back to closed position (0 degrees around the Y-axis)
                targetRotation = originalRotation;
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

        public Vector3 GetCenterPoint()
        {
            if (doorRenderer != null)
            {
                // Vector pointing from the door to the player
                Vector3 directionToPlayer = (PlayerManager.Instance.transform.position - transform.position).normalized;

                // Get the door's forward direction
                Vector3 doorForward = transform.forward;

                // Calculate the dot product to determine the relative position of the player
                float dotProduct = Vector3.Dot(directionToPlayer, doorForward);

                // If the player is on the side of the door that is "in front" of the door (based on the door's forward direction)
                if (dotProduct > 0)
                {
                    // Player is in front of the door, use the center from the doorRenderer
                    return doorRenderer.bounds.center;
                }
                else
                {
                    // Player is behind the door, adjust the center to the opposite side
                    Vector3 adjustedCenter = doorRenderer.bounds.center - doorRenderer.bounds.extents.z * doorForward;
                    return adjustedCenter;
                }
            }

            // Fallback to the transform position if no Renderer is found
            return transform.position;
        }

    }

}