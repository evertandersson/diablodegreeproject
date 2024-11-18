using Events;
using UnityEngine;

namespace Game
{
    public class MainMenu : EventHandler.GameEventBehaviour
    {
        private void OnEnable()
        {
            EventHandler.Main.PushEvent(this);
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}
