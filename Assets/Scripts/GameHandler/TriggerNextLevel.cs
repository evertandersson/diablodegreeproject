using UnityEngine;

namespace Game
{
    public class TriggerNextLevel : MonoBehaviour, Interactable
    {
        [SerializeField] private string sceneName;
        public static string SceneNameTransitioningTo { get; private set; }

        public static bool isTransitioningToNextLevel = false;

        public Vector3 GetCenterPoint()
        {
            return transform.position;
        }

        public void Trigger()
        {
            isTransitioningToNextLevel = true;
            SceneNameTransitioningTo = sceneName;
            SaveManager.Instance.Save();
            LevelTransition.Instance.Load(sceneName, null);
        }
    }

}
