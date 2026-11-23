using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 0.2f;

    public Vector3 positionOffset = new Vector3(0, 0, -10);

    private void Awake()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Check Player tag.");
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition =
            target.position + positionOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}


