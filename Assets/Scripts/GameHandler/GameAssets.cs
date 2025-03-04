using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets Instance
    {
        get
        {
            if (instance == null) instance = Instantiate(Resources.Load<GameAssets>("Prefabs/Necessary Game Managers/GameAssets"));
            return instance;
        }
    }

    public Transform damagePopup;
}
