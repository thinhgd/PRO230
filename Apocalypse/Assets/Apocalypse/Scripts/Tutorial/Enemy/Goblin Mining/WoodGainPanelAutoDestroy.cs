using UnityEngine;

public class WoodGainPanelAutoDestroy : MonoBehaviour
{
    public float lifeTime = 1.5f;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
