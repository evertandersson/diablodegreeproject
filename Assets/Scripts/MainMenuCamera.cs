using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    private Vector3 targetPos = new Vector3(-17.59f, 1.69f, -1.19f);
    private Vector3 targetRot = new Vector3(0, 90, 0);

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.01f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), 0.01f);
    }
}
