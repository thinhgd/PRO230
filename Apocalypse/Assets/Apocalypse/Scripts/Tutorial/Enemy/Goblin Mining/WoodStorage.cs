using UnityEngine;

public class WoodStorage : MonoBehaviour
{
    public static WoodStorage Instance;
    public Transform storagePoint;

    void Awake()
    {
        Instance = this;
    }
}
