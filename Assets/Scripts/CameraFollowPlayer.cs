using Game;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float rotateSpeed = 100f;

    private void Update()
    {
        transform.position = PlayerManager.Instance.transform.position;

        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = 1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
}
