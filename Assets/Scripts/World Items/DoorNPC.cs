using UnityEngine;

namespace Game
{
    public class DoorNPC : Door
    {
        public void TriggerByNPC()
        {
            state = State.Closed;

            Vector3 offset = new Vector3(0, 1, -0.5f);
            Vector3 textSpawnPosition = GetCenterPoint() + offset;

            PopupText text = ObjectPooling.Instance.SpawnFromPool("PopupText", textSpawnPosition, Quaternion.identity).GetComponent<PopupText>();
            text.message = "Unlocked door";
            text.StartCoroutine("Trigger");
            SoundManager.PlaySound(SoundType.DOOR);

            SaveManager.Instance.AddObjectToList(id, SaveManager.Instance.doorsOpenedList);
        }
    }
}
