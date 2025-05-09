using System.Collections;
using UnityEngine;

public class RandomWalker : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveRadius = 3f;
    public float minRestTime = 1f;
    public float maxRestTime = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private bool isMoving = true;
    public bool IsMoving => isMoving;

    void Start()
    {
        startPos = transform.position;
        SetNewTarget();
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                isMoving = false;
                StartCoroutine(RestBeforeNextMove());
            }
        }
    }

    IEnumerator RestBeforeNextMove()
    {
        float restTime = Random.Range(minRestTime, maxRestTime);
        yield return new WaitForSeconds(restTime);
        SetNewTarget();
        isMoving = true;
    }

    void SetNewTarget()
    {
        float x = Random.Range(-moveRadius, moveRadius);
        float y = Random.Range(-moveRadius, moveRadius);
        targetPos = new Vector3(startPos.x + x, startPos.y + y, transform.position.z);
    }
}
