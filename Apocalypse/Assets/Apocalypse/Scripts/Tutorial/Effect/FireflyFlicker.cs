using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireflyFlicker : MonoBehaviour
{
    public Light2D light2D;

    public float minRadius = 0.1f;
    public float maxRadius = 1.0f;
    public float flickerSpeed = 1.5f;

    private float randomOffset;

    void Start()
    {
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Mathf.PerlinNoise((Time.time + randomOffset) * flickerSpeed, 0f);
        float flickerRadius = Mathf.Lerp(minRadius, maxRadius, t);

        if (light2D != null)
        {
            light2D.pointLightOuterRadius = flickerRadius;
            light2D.intensity = flickerRadius / maxRadius;
        }
    }
}
