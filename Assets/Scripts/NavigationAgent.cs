using UnityEngine;
using UnityEngine.AI;

public class NavigationAgent : MonoBehaviour
{

    Vector3 _target;
    public Vector3 Target
    {
        get { return _target; }
        set
        {
            _target = value;
            CalculatePath();
        }
    }

    [Header("Settings")]
    public float Speed = 3f;

    NavMeshPath navMeshPath;
    int pathIndex = 0;

    void Start()
    {
        navMeshPath = new NavMeshPath();
    }

    void Update()
    {
        if (pathIndex < navMeshPath.corners.Length)
        {
            transform.position = Vector3.MoveTowards(transform.position, navMeshPath.corners[pathIndex], Speed * Time.deltaTime);

            if(transform.position == navMeshPath.corners[pathIndex])
            {
                pathIndex++;
            }
        }
    }

    public void CalculatePath()
    {
        if (NavMesh.CalculatePath(transform.position, Target, NavMesh.AllAreas, navMeshPath))
        {
            pathIndex = 0;
        }
    }

    #region Debug
    private void OnGUI()
    {
        if (navMeshPath != null)
        {
            Rect r = new Rect(0, 0, 300, 300);
            GUI.Label(r, "Path status: " + navMeshPath.status);
            r.y += 30;
            GUI.Label(r, "Corners: " + navMeshPath.corners.Length);
        }
    }

    private void OnDrawGizmos()
    {
        if (navMeshPath != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < navMeshPath.corners.Length; i++)
            {
                Gizmos.DrawWireSphere(navMeshPath.corners[i], .2f);
                if (i > 0)
                    Gizmos.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i - 1]);
            }
        }
    }
    #endregion
}
