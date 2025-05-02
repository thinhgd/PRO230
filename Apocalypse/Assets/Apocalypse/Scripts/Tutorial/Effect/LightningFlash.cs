using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningFlash : MonoBehaviour
{
    public Light2D flashLight;
    public AudioSource thunderSound;
    public AudioClip thunderClip;
    public AudioClip rainClip;
    public float minDelay = 5f;
    public float maxDelay = 15f;

    private void Start()
    {   
        thunderSound.PlayOneShot(rainClip);
        StartCoroutine(LightningRoutine());
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            thunderSound.PlayOneShot(thunderClip);

            yield return StartCoroutine(DoLightningFlash());

        }
    }

    IEnumerator DoLightningFlash()
    {
        flashLight.intensity = 2f;
        yield return new WaitForSeconds(0.1f);
        flashLight.intensity = 0f;

        if (Random.value > 0.5f)
        {
            yield return new WaitForSeconds(0.1f);
            flashLight.intensity = 1.2f;
            yield return new WaitForSeconds(0.05f);
            flashLight.intensity = 0f;
        }
    }
}
