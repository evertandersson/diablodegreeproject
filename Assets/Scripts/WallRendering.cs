using Game;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallRendering : MonoBehaviour
{
    private List<Renderer> wallRenderers;

    private bool isQuitting = false;

    private void Start()
    {
        // Fetch all Renderer components in child GameObjects and filter for the "Wall" layer
        wallRenderers = new List<Renderer>();
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.gameObject.layer == LayerMask.NameToLayer("Wall") ||
                renderer.gameObject.CompareTag("Hidable"))
            {
                wallRenderers.Add(renderer);
            }
        }
        OnPlayerYValueChanged(PlayerManager.Instance.transform.position.y);

        Application.quitting += OnApplicationQuit;
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnYValueChanged += OnPlayerYValueChanged;
        }
    }

    private void OnDisable()
    {
        Application.quitting -= OnApplicationQuit;
        if (!isQuitting && PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnYValueChanged -= OnPlayerYValueChanged;
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnPlayerYValueChanged(float playerY)
    {
        foreach (Renderer renderer in wallRenderers)
        {
            renderer.enabled = renderer.transform.position.y - 3 <= playerY;

            // Make hidable ground walkable if rendered
            if (!renderer.gameObject.CompareTag("Hidable")) 
                continue;
            
            if (renderer.enabled)
            {
                renderer.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                renderer.gameObject.layer = LayerMask.NameToLayer("Wall");
            }
        }
    }
}
