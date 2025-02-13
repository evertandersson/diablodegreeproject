using Game;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float rotateSpeed = 100f;

    private float switchTargetSpeed = 2f;

    private void Update()
    {
        if (EventHandler.Main.CurrentEvent is not DialougeManager)
        {
            transform.position = PlayerManager.Instance.transform.position;

            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotateDir = 1f;
            if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

            transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
        }
        else
        {
            if (DialougeManager.Instance.currentNPC != null)
            {
                transform.position = Vector3.Lerp(transform.position, DialougeManager.Instance.currentNPC.transform.position, switchTargetSpeed * Time.deltaTime);

                Vector3 direction = (DialougeManager.Instance.currentNPC.transform.position - PlayerManager.Instance.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, switchTargetSpeed * Time.deltaTime);
            }
        }
    }
}
