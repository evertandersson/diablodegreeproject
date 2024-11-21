using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Game
{
    public class OptionsMenu : Popup
    {
        public override void OnEnd()
        {
            base.OnEnd();
            Destroy(gameObject);
        }
    }

}