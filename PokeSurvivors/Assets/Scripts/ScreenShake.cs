/*using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        originalPos = transform.localPosition; // Record current position exactly when called
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        // When nested, the local position should stay around zero
        Vector3 startPos = new Vector3(0, 0, 0);

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Move the camera locally inside the Cinemachine 'frame'
            transform.localPosition = new Vector3(startPos.x + offsetX, startPos.y + offsetY, startPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}*/