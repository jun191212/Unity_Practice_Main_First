using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSize = 30f;
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private bool autoFindPlayer = true;

    private Camera minimapCam;

    void Start()
    {
        minimapCam = GetComponent<Camera>();
        minimapCam.orthographicSize = cameraSize;

        if (autoFindPlayer && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // 플레이어 따라가지 않을 때 맵 중심으로 카메라
        if (!followPlayer)
        {
            transform.position = new Vector3(0, 0, -20);
        }
    }

    void LateUpdate()
    {
        if (followPlayer && player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = -20;
            transform.position = newPosition;
        }
    }
}