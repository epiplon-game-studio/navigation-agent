using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavigationAgent : MonoBehaviour
{
    Vector3 _target;
    public Vector3 Target
    {
        get { return _target; }
        private set
        {
            _target = value;
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

    public Vector3 Velocity { get; private set; }
    public Vector3 AngularVelocity { get; private set; }

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
                    {
                        Vector3 nextPosition = Vector3.MoveTowards(transform.position, target, m_positionSpeed * Time.deltaTime);
                        Velocity = nextPosition - transform.position;
                        transform.position = nextPosition;
                    }
                }
                else
                {
                    Vector3 nextPosition = Vector3.MoveTowards(transform.position, target, m_positionSpeed * Time.deltaTime);
                    Velocity = nextPosition - transform.position;
                    transform.position = nextPosition;
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
            else
            {
                Velocity = Vector3.zero;
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

    public void SetDestination(Vector3 destination)
    {
        Target = destination;

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
        var targetRotation = Quaternion.LookRotation(m_direction, Vector3.up);

        var nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        AngularVelocity = nextRotation.eulerAngles - transform.rotation.eulerAngles;
        transform.rotation = nextRotation;
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
            Rect r = new Rect(0, 0, 300, 600);
            GUI.Label(r, "Velocity X: " + Velocity.x);
            r.y += 30;
            GUI.Label(r, "Velocity Y: " + Velocity.y);
            r.y += 30;
            GUI.Label(r, "Velocity Z: " + Velocity.z);
            r.y += 60;
            GUI.Label(r, "Angular Velocity X: " + AngularVelocity.x);
            r.y += 30;
            GUI.Label(r, "Angular Velocity Y: " + AngularVelocity.y);
            r.y += 30;
            GUI.Label(r, "Angular Velocity Z: " + AngularVelocity.z);
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
