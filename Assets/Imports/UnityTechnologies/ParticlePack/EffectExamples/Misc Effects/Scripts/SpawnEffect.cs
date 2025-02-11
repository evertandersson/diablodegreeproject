using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public float spawnEffectTime = 2f;
    public float pause = 1f;
    public AnimationCurve fadeIn;

    public bool despawn = false;

    private ParticleSystem ps;
    private float timer = 0f;
    private Renderer _renderer;
    private int shaderProperty;
    private bool hasStartedDissolving = false; // New flag to prevent looping

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;
    }

    void Update()
    {
        if (despawn && !hasStartedDissolving) // Start dissolve only once
        {
            hasStartedDissolving = true; // Mark dissolve as started
            ps.Play(); // Play dissolve effect
        }

        if (hasStartedDissolving)
        {
            if (timer < spawnEffectTime)
            {
                timer += Time.deltaTime;
                _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
            }
            else
            {
                Destroy(gameObject); // Remove the enemy after dissolve
            }
        }
    }
}
