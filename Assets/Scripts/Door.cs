using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game
{
    public class Door : MonoBehaviour, Interactable
    {
        [SerializeField] private ItemSO key;
        Renderer doorRenderer;

        public enum State
        {
            Locked,
            Closed,
            Open
        }

        public State state;


        [SerializeField] private string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            doorRenderer = GetComponent<Renderer>();
            SaveManager.LoadWorldObjects += Load;
        }
        private void OnDisable()
        {
            SaveManager.LoadWorldObjects -= Load;
        }

        private void Load()
        {
            if (SaveManager.Instance.openedDoorsList.serializableList.Exists(door => door.name == id))
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
            foreach (InventorySlot slot in PlayerManager.Instance.inventory.inventory)
            {
                if (slot.item != null)
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

                        SaveManager.Instance.AddObjectToList(id, SaveManager.Instance.openedDoorsList);

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

        public Vector3 GetCenterPoint()
        {
            if (doorRenderer != null)
            {
                return doorRenderer.bounds.center; // Center of the door in world space
            }

            // Fallback to the transform position if no Renderer is found
            return transform.position;
        }
    }

}