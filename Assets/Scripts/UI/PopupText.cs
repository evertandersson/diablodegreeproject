using System.Collections;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour, IPooledObject
{
    private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI text;
    public string message;
    private Vector3 startPosition;

    [SerializeField] private float fadeSpeed = 2f; // Speed of fading
    [SerializeField] private float moveSpeed = 0.5f; // Speed of upward movement

    void Start()
    {
        cameraTransform = Camera.main.transform;
        startPosition = transform.position;

        if (cameraTransform == null)
        {
            Debug.LogError("No camera found in the scene!");
        }
    }

    public void OnObjectSpawn()
    {
        text.alpha = 0;
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Ensure the text faces the camera
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }

    public IEnumerator Trigger()
    {
        // Fade in and move up
        while (text.alpha < 1)
        {
            text.text = message; // Ensure the message is set
            text.alpha += fadeSpeed * Time.deltaTime;
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // Pause briefly while continuing to move up
        float pauseTime = 1f;
        while (pauseTime > 0)
        {
            pauseTime -= Time.deltaTime;
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // Fade out and move up
        while (text.alpha > 0)
        {
            text.alpha -= fadeSpeed * Time.deltaTime;
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // Reset position or handle pooling cleanup
        transform.position = startPosition; // Reset to start position if reusing
    }



}
