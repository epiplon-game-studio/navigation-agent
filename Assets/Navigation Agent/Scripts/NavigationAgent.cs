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

    [HideInInspector] public bool m_enabled;

    [HideInInspector] public float m_positionSpeed = 3f;
    [HideInInspector] public bool axisX, axisY, axisZ;

    [Header("ROTATION")]
    public float m_rotationSpeed = 180f;

    NavMeshPath navMeshPath;
    int pathIndex = 0;
    Vector3 m_direction;

    // follow at axis

    void Start()
    {
        navMeshPath = new NavMeshPath();
    }

    void Update()
    {
        if (m_enabled)
        {
            if (pathIndex < navMeshPath.corners.Length)
            {
                var target = BuildAxisTarget(transform.position);

                //TODO: update rigidbody?
                transform.position = Vector3.MoveTowards(transform.position, target, m_positionSpeed * Time.deltaTime);

                if(transform.position == target)
                {
                    pathIndex++;
                }
                else
                {
                    // update the direction while on path
                    m_direction = (target - transform.position).normalized;
                }
            }

            if (m_direction != Vector3.zero)
                RotateAgent();
        }
    }

    public void CalculatePath()
    {
        if (NavMesh.CalculatePath(transform.position, Target, NavMesh.AllAreas, navMeshPath))
        {
            pathIndex = 0;
        }
    }

    /// <summary>
    /// Build a target position based on which axis it will be updated
    /// </summary>
    /// <param name="origin">Original position of the agent</param>
    /// <returns>The resulting target</returns>
    Vector3 BuildAxisTarget(Vector3 origin)
    {
        Vector3 target = origin;
        target.x = axisX ? navMeshPath.corners[pathIndex].x : origin.x;
        target.y = axisY ? navMeshPath.corners[pathIndex].y : origin.y;
        target.z = axisZ ? navMeshPath.corners[pathIndex].z : origin.z;
        return target;
    }

    void RotateAgent()
    {
        var rotation = Quaternion.LookRotation(m_direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, m_rotationSpeed * Time.deltaTime);
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
