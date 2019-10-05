using UnityEngine;

namespace vnc.AI.Samples
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public NavigationAgent navigationAgent;
        public LayerMask hitMask;
        public bool followMouse = true;
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
            if (followMouse)
            {
                ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
                {
                    navigationAgent.SetDestination(hit.point);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ray = _camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
                    {
                        navigationAgent.SetDestination(hit.point);
                    }
                }
            }


        }
    }
}