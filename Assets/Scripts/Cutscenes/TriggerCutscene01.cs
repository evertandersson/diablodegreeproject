using System;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene01 : TriggerCutscene
{
    public static event Action StartCutscene01;
    public static event Action StopCutscene01;

    protected override Action StartCutsceneAction => StartCutscene01;
    protected override Action StopCutsceneAction => StopCutscene01;
}
