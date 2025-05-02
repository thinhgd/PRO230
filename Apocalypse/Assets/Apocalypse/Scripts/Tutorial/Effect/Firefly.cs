using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    Vector3 target;

    void Start() {
        SetNewTarget();
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.1f) SetNewTarget();
    }

    void SetNewTarget() {
        float radius = 3f;
        target = new Vector3(
            transform.position.x + Random.Range(-radius, radius),
            transform.position.y + Random.Range(-radius, radius),
            transform.position.z
        );
    }
}
