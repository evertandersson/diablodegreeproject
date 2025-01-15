using Game;
using UnityEngine;

public abstract class Loadable : MonoBehaviour
{
    [SerializeField] public string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    protected abstract void Load();

    protected virtual void Awake()
    {
        SaveManager.LoadWorldObjects += Load;
    }

    protected virtual void OnDisable()
    {
        SaveManager.LoadWorldObjects -= Load;
    }
}
