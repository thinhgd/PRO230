// using UnityEngine;
// using Pathfinding;

// namespace Game.Tutorials
// {
//     public class DigEnemy : BaseEnemy
//     {
//         [Header("A* Settings")]
//         public Transform targetCastle;
//         public float nextWaypointDistance = 0.5f;
//         public LayerMask wallLayer;
//         public float digDelay = 1f;

//         private Path path;
//         private int currentWaypoint = 0;
//         private Seeker seeker;
//         private bool isDigging = false;

//         protected override void Start()
//         {
//             base.Start();
//             seeker = GetComponent<Seeker>();

//             if (targetCastle == null)
//             {
//                 FindNearestCastle();
//                 if (targetCastle == null)
//                 {
//                     Debug.LogError("Không tìm thấy castle gần nhất!");
//                     return;
//                 }
//             }
//             Debug.Log("[DIG ENEMY] Start() chạy!");
//             InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
//         }

//         void FindNearestCastle()
//         {
//             float shortestDistance = Mathf.Infinity;
//             GameObject[] castles = GameObject.FindGameObjectsWithTag("Town");

//             foreach (GameObject castle in castles)
//             {
//                 float distance = Vector2.Distance(transform.position, castle.transform.position);
//                 if (distance < shortestDistance)
//                 {
//                     shortestDistance = distance;
//                     targetCastle = castle.transform;
//                 }
//             }
//         }

//         void UpdatePath()
//         {   
//             if (seeker.IsDone() && targetCastle != null)
//             {
//                 seeker.StartPath(rb.position, targetCastle.position, OnPathComplete);
//             }
//             Debug.Log("[DIG ENEMY] UpdatePath chạy");
//         }

//         void OnPathComplete(Path p)
//         {
//             if (!p.error)
//             {
//                 path = p;
//                 currentWaypoint = 0;
//             }
//         }

//         void Update()
//         {
//             Vector2 testTarget = rb.position + Vector2.right * 3f;
//             RaycastHit2D hit = Physics2D.Linecast(rb.position, testTarget, wallLayer);
//             Debug.DrawLine(rb.position, testTarget, Color.red);
//             if (hit.collider != null)
//             {
//                 Debug.Log("Thử nghiệm: Gặp tường " + hit.collider.name);
//             }
//         }
//         protected override void Run()
//         {
//             Debug.Log("[DIG ENEMY] Đang chạy Run()");
//             if (path == null || isDigging) return;

//             if (currentWaypoint >= path.vectorPath.Count)
//             {
//                 SetState(EnemyState.Attack);
//                 rb.velocity = Vector2.zero;
//                 return;
//             }

//             Vector2 nextPoint = path.vectorPath[currentWaypoint];
//             Vector2 direction = (nextPoint - rb.position).normalized;

//             // Raycast phía trước để kiểm tra tường
//             RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 1f, wallLayer);
//             if (hit.collider != null)
//             {
//                 Debug.Log("Đã phát hiện tường: " + hit.collider.name + " tại vị trí " + hit.point);
//                 StartCoroutine(DigThroughWall(hit, direction));
//                 return;
//             }

//             Vector2 force = direction * stats.moveSpeed * Time.deltaTime;
//             rb.velocity = force;

//             float distance = Vector2.Distance(rb.position, nextPoint);
//             if (distance < nextWaypointDistance)
//             {
//                 currentWaypoint++;
//             }
//             Debug.DrawLine(rb.position, nextPoint, Color.red);
//         }

//         System.Collections.IEnumerator DigThroughWall(RaycastHit2D wallHit, Vector2 direction)
//         {
//             isDigging = true;
//             rb.velocity = Vector2.zero;
//             SetState(EnemyState.Idle);

//             // Ẩn enemy
//             GetComponent<SpriteRenderer>().enabled = false;
//             GetComponent<Collider2D>().enabled = false;

//             yield return new WaitForSeconds(digDelay);

//             // 👉 Dịch chuyển theo normal của mặt tường để tránh kẹt trong tường
//             Vector2 newPosition = wallHit.point + wallHit.normal * 2f;
//             Debug.Log("Teleport từ: " + transform.position + " đến: " + newPosition);
//             transform.position = newPosition;

//             // Hiện lại enemy
//             GetComponent<SpriteRenderer>().enabled = true;
//             GetComponent<Collider2D>().enabled = true;

//             isDigging = false;
//             SetState(EnemyState.Run);
//         }

//         private void OnDrawGizmosSelected()
//         {
//             if (path != null && currentWaypoint < path.vectorPath.Count)
//             {
//                 Gizmos.color = Color.red;
//                 Gizmos.DrawLine(transform.position, path.vectorPath[currentWaypoint]);
//             }
//         }
//     }
// }
