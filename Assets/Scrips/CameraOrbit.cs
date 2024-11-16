using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad,0);
    private new Camera camera;
    private Vector2 nearPlaneSize;

    public Transform follow;
    public float maxDistance;
    public Vector2 sensitivity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();
        CalculateNearPlaneSize();
    }

    void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        if (h != 0) {
            angle.x += h * Mathf.Deg2Rad * sensitivity.x; 
        }

        if (v != 0)
        {
            angle.y += v * Mathf.Deg2Rad * sensitivity.y;
            angle.y = Mathf.Clamp(angle.y, -80 * Mathf.Deg2Rad, 80 * Mathf.Deg2Rad);
        }
    }


    private void CalculateNearPlaneSize() {
        float h = Mathf.Tan(camera.fieldOfView*Mathf.Deg2Rad/2)*camera.nearClipPlane;
        float w = h * camera.aspect;

        nearPlaneSize = new Vector2(w, h);
    }

    private Vector3[] GetCameraCollisionPoints(Vector3 direction) {
        Vector3 position = follow.position;
        Vector3 center = position + direction * (camera.nearClipPlane + 0.2f);

        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;
            
        return new Vector3[] { 
            center - right + up, 
            center + right + up,
            center - right - up,
            center + right - up
        };
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = new Vector3(Mathf.Cos(angle.x) * Mathf.Cos(angle.y),-Mathf.Sin(angle.y), -Mathf.Sin(angle.x) * Mathf.Cos(angle.y));

        RaycastHit hit;
        float distance = maxDistance;
        Vector3[] points =  GetCameraCollisionPoints(direction);

        foreach (Vector3 point in points)
        {
            if (Physics.Raycast(point, direction, out hit, maxDistance))
            {
                distance = Mathf.Min((hit.point - follow.position).magnitude,distance);
            }
        }

        transform.position = follow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
    }
}
