using System.Collections.Generic;
using UnityEngine;

public class Procedural : MonoBehaviour
{
    public List<Transform> chains = new List<Transform>();

    [SerializeField] float maxDistance;

    [SerializeField] float speed;

    private void LateUpdate()
    {
        for (int i = 0; i < chains.Count - 1; i++)
        {
            ApplyDistanceConstraint(chains[i + 1], chains[i]);
        }
    }

    void ApplyDistanceConstraint(Transform a, Transform b)
    {
        Vector3 dirBToA = (a.position - b.position).normalized * maxDistance;
        a.position = b.position + dirBToA;
    }
}
