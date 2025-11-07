using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;

    void Start()
    {
        if (autoFindPlayer && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
      
            }
     
        }
    }

    void LateUpdate()
    {
        if (target == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            return;
        }

        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        smoothPosition.z = offset.z;
        transform.position = smoothPosition;
    }
}