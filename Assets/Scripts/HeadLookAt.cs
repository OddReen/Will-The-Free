using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class HeadLookAt : MonoBehaviour
{
    [SerializeField] RigBuilder rigBuilder;

    [SerializeField] MultiAimConstraint headAimConstraint;

    private void Start()
    {
        var constraintData = headAimConstraint.data;
        var sources = constraintData.sourceObjects;

        sources.Add(new WeightedTransform(Camera.main.transform, 1.0f));

        constraintData.sourceObjects = sources;
        headAimConstraint.data = constraintData;

        rigBuilder.Build();
    }
}
