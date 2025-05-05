using UnityEngine;

namespace Game.Tutorials
{
    public class TowerRaycast : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float radius = 3f;
        [SerializeField] private float angle = 10 * Mathf.Deg2Rad;

        void Update()
        {
            for(int i = 0; i < 36; ++i)
            {
                Debug.DrawRay(this.transform.position, new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0) * radius, Color.yellow);
            }
        }

        void FixedUpdate()
        {
            RaycastHit2D hit = Physics2D.CircleCast(new Vector2(this.transform.position.x, this.transform.position.y), radius, Vector2.zero, 3f, enemyLayer);
            if(hit.collider != null)
            {
                Debug.Log("Find enemy");
            }
            else if(hit.collider == null)
            {
                Debug.Log("You're safe now!!");
            }
        }
    }
}