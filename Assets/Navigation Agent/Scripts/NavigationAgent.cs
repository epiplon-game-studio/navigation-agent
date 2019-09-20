using System;
using System.Collections;
using System.Reflection;
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

    public bool m_enabled;

    // POSITION
    public float m_positionSpeed = 3f;
    public bool axisX, axisY, axisZ;

    // ROTATION
    public float m_rotationSpeed = 180f;
    public RotationStyle rotationStyle;

    // ADVANCED
    public RotationPrecision rotationPrecision = RotationPrecision.High;
    public float repathDelay = 0.1f;
    public int navmeshAreas;

    public bool IsStopped
    {
        get
        {
            return pathIndex == navMeshPath.corners.Length;
        }
    }

    NavMeshPath navMeshPath;
    int pathIndex = 0;
    Vector3 m_direction;
    [SerializeField] bool advancedOptions;

    void Start()
    {
        navMeshPath = new NavMeshPath();
        _target = transform.position;
        StartCoroutine(AutoRepath());
    }

    void Update()
    {
        if (m_enabled)
        {
            if (pathIndex < navMeshPath.corners.Length)
            {
                var target = NextPosition();

                //TODO: update rigidbody?
                if (rotationStyle == RotationStyle.RotateBeforeMoving)
                {
                    bool isFacingTarget = IsFacingTarget();
                    if (isFacingTarget)
                        transform.position = Vector3.MoveTowards(transform.position, target, m_positionSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, m_positionSpeed * Time.deltaTime);
                }

                if (transform.position == target)
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
            {
                switch (rotationStyle)
                {
                    case RotationStyle.RotateBeforeMoving:
                    case RotationStyle.Always:
                        RotateAgent();
                        break;
                    case RotationStyle.MovingOnly:
                        if (!IsStopped)
                            RotateAgent();
                        break;
                }
            }
        }
    }

    IEnumerator AutoRepath()
    {
        while (true)
        {
            if(m_enabled)
            {
                CalculatePath();
                yield return new WaitForSecondsRealtime(repathDelay);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void CalculatePath()
    {
        if (NavMesh.CalculatePath(transform.position, Target, navmeshAreas, navMeshPath))
        {
            pathIndex = 0;
        }
    }

    /// <summary>
    /// Build a target position based on which axis it will be updated
    /// </summary>
    /// <returns>The resulting target</returns>
    public Vector3 NextPosition()
    {
        Vector3 target = transform.position;
        target.x = axisX ? navMeshPath.corners[pathIndex].x : transform.position.x;
        target.y = axisY ? navMeshPath.corners[pathIndex].y : transform.position.y;
        target.z = axisZ ? navMeshPath.corners[pathIndex].z : transform.position.z;
        return target;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsFacingTarget()
    {
        int precisionDigits = (int)rotationPrecision;
        // compare facing the target depending on selected using axis
        bool faceX = axisX ? Math.Round(m_direction.x, precisionDigits) == Math.Round(transform.forward.x, precisionDigits) : true;
        bool faceY = axisY ? Math.Round(m_direction.y, precisionDigits) == Math.Round(transform.forward.y, precisionDigits) : true;
        bool faceZ = axisZ ? Math.Round(m_direction.z, precisionDigits) == Math.Round(transform.forward.z, precisionDigits) : true;
        return faceX && faceY && faceZ;
    }

    void RotateAgent()
    {
        var rotation = Quaternion.LookRotation(m_direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, m_rotationSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        StopCoroutine(AutoRepath());
    }

    #region Debug
    private void OnGUI()
    {
        if (navMeshPath != null)
        {
            Rect r = new Rect(0, 0, 300, 300);
            GUI.Label(r, "Facing Target: " + IsFacingTarget());
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


    public enum RotationStyle
    {
        Never,      // no rotation at all
        Always,     // always
        MovingOnly, // no rotation when stopped
        RotateBeforeMoving
    }

    public enum RotationPrecision { High = 0, Medium = 1, Low = 2, VeryLow = 3 }
}
