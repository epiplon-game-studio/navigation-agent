using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public NavigationAgent navigationAgent;
    public LayerMask hitMask;
    Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        SetTarget();
    }

    Ray ray;
    void SetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
            {
                navigationAgent.Target = hit.point;
            }
        }
    }
}
