using TMPro;
using UnityEngine;

public class FloatingWoodUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifetime = 1.5f;
    public Vector3 moveOffset = new Vector3(0, 50f, 0);
    public float moveSpeed = 30f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, targetPosition, timer / lifetime);

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    public void SetText(string value)
    {
        if (text != null)
            text.text = value;
    }
}
