using UnityEngine;

[RequireComponent(typeof(Transform))]
public class DampedSpringParentAware : MonoBehaviour
{
    public enum SpaceMode { Local, World }

    [Header("Spring settings")]
    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;
    public SpaceMode mode = SpaceMode.Local;

    [Header("Target")]
    public bool useTargetTransform = false;
    public Transform targetTransform;             // used if useTargetTransform = true
    public Vector3 targetPosition = Vector3.zero; // interpreted in local or world space depending on mode

    [Header("Parent handling")]
    public bool resetVelocityOnParentChange = true; // recommended true to avoid glitches

    // internal state (kept in the same space as mode)
    private Vector3 velocity = Vector3.zero;
    private DampedSpringMotionParams motionParams;

    private Transform cachedParent;

    void Start()
    {
        cachedParent = transform.parent;
        // init params once; we'll recalc each Update (since Time.deltaTime changes)
        DampedSpring.CalcDampedSpringMotionParams(ref motionParams, Time.deltaTime, angularFrequency, dampingRatio);
    }

    void Update()
    {
        // recompute params (because deltaTime changes)
        DampedSpring.CalcDampedSpringMotionParams(ref motionParams, Time.deltaTime, angularFrequency, dampingRatio);

        // detect parent change
        if (transform.parent != cachedParent)
        {
            OnParentChanged(cachedParent, transform.parent);
            cachedParent = transform.parent;
        }

        if (mode == SpaceMode.Local)
        {
            // equilibrium in local space
            Vector3 equilibrium = useTargetTransform && targetTransform != null
                ? transform.parent == targetTransform.parent // if same parent, use target's local
                    ? targetTransform.localPosition
                    : transform.InverseTransformPoint(targetTransform.position) // convert world target to this local space
                : targetPosition;

            Vector3 pos = transform.localPosition;
            UpdateVec3(ref pos, ref velocity, equilibrium, motionParams);
            transform.localPosition = pos;
        }
        else // World
        {
            Vector3 equilibrium = useTargetTransform && targetTransform != null
                ? targetTransform.position
                : targetPosition;

            Vector3 pos = transform.position;
            UpdateVec3(ref pos, ref velocity, equilibrium, motionParams);
            transform.position = pos;
        }
    }

    private void OnParentChanged(Transform oldParent, Transform newParent)
    {
        if (!resetVelocityOnParentChange) return;

        // simplest robust approach: zero velocity when the parent changes.
        // This prevents spikes when local axes or parent's movement would otherwise corrupt the spring state.
        velocity = Vector3.zero;
    }

    // helper: apply scalar spring to each component separately
    private void UpdateVec3(ref Vector3 pos, ref Vector3 vel, Vector3 equilibrium, DampedSpringMotionParams p)
    {
        float px = pos.x, vx = vel.x;
        DampedSpring.UpdateDampedSpringMotion(ref px, ref vx, equilibrium.x, in p);
        pos.x = px; vel.x = vx;

        float py = pos.y, vy = vel.y;
        DampedSpring.UpdateDampedSpringMotion(ref py, ref vy, equilibrium.y, in p);
        pos.y = py; vel.y = vy;

        float pz = pos.z, vz = vel.z;
        DampedSpring.UpdateDampedSpringMotion(ref pz, ref vz, equilibrium.z, in p);
        pos.z = pz; vel.z = vz;
    }

    // optional inspector helper to set world target quickly
    public void SetWorldTarget(Vector3 worldPos)
    {
        useTargetTransform = false;
        targetPosition = worldPos;
        if (mode == SpaceMode.Local && transform.parent != null)
        {
            // convert world to local
            targetPosition = transform.parent.InverseTransformPoint(worldPos);
        }
    }
}
