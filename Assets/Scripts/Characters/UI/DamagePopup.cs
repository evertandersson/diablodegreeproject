using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DamagePopup : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 0.5f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private Transform cameraTransform;

    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.Instance.damagePopup, position, Quaternion.identity);
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.SetUp(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private static int sortingOrder;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        cameraTransform = Camera.main.transform;
    }

    public void SetUp(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit)
        {
            // If NOT critical hit
            textMesh.fontSize = 10;
            textColor = Color.white;
        }
        else
        {
            // If critical hit
            textMesh.fontSize = 15;
            textColor = Color.red;
        }

        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(0.5f, 1) * 4f;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 0.4f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Ensure the text faces the camera
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }

}
