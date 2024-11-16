using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Vector3 mouseInputPosition;

    [SerializeField]
    private Vector3 offset;

    GameObject player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().gameObject;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.MaxValue))
        {
            mouseInputPosition = hit.point;
        }

        transform.position = player.transform.position + offset;
    }
}
