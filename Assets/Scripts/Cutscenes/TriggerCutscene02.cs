using System;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene02 : TriggerCutscene
{
    public static event Action StartCutscene02;
    public static event Action StopCutscene02;

    protected override Action StartCutsceneAction => StartCutscene02;
    protected override Action StopCutsceneAction => StopCutscene02;
}
