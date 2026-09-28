using System.Collections;
using UnityEngine;

/// <summary>
/// Camera shake rotation đơn giản cho 2D.
/// - Gắn script này lên Camera.
/// - Camera sẽ rung bằng cách xoay quanh trục Z.
/// - Gọi ShakeCam.Instance.Shake(0.5f, 3f) để rung.
/// </summary>
public class ShakeCam : MonoBehaviour
{
    public static ShakeCam Instance;

    [Header("Thiết lập mặc định")]
    [Tooltip("Thời gian rung (giây)")]
    public float defaultDuration = 0.4f;

    [Tooltip("Góc xoay tối đa quanh trục Z")]
    public float defaultMagnitude = 3f;

    [Tooltip("Nếu true thì dùng Perlin noise cho chuyển động mượt hơn")]
    public bool usePerlin = true;

    [Tooltip("Tốc độ cho Perlin noise")]
    public float perlinSpeed = 20f;

    private Quaternion originalRotation;
    private Coroutine shakeRoutine;
    private float lastMagnitude;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        originalRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        originalRotation = transform.localRotation;
    }

    private void OnDisable()
    {
        transform.localRotation = originalRotation;
    }

    /// <summary>
    /// Gọi để bắt đầu rung với mặc định.
    /// </summary>
    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    /// <summary>
    /// Gọi để bắt đầu rung với thời gian và góc xoay cụ thể.
    /// </summary>
    /// <param name="duration">Thời gian rung (giây)</param>
    /// <param name="magnitude">Góc xoay tối đa theo độ</param>
    public void Shake(float duration = 0.2f, float magnitude = 2f)
    {
        if (magnitude < lastMagnitude)
            return;

        lastMagnitude = magnitude;

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    /// <summary>
    /// Dừng rung lập tức và đưa camera về rotation ban đầu.
    /// </summary>
    public void StopShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        transform.localRotation = originalRotation;
        lastMagnitude = 0f;
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        float seed = Random.Range(0f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1f - Mathf.Clamp01(percentComplete);

            float rotationZ;

            if (usePerlin)
            {
                float noise = Mathf.PerlinNoise(seed, Time.unscaledTime * perlinSpeed);
                noise = (noise - 0.5f) * 2f;

                rotationZ = noise * magnitude * damper;
            }
            else
            {
                rotationZ = Random.Range(-1f, 1f) * magnitude * damper;
            }

            transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationZ);

            yield return null;
        }

        transform.localRotation = originalRotation;
        shakeRoutine = null;
        lastMagnitude = 0f;
    }

    //-------------------- Time Stop --------------------//

    private bool waiting = false;

    public void Stop(float duration, float timeScale)
    {
        if (duration <= 0f || timeScale <= 0f)
            return;

        if (waiting)
            return;

        Time.timeScale = timeScale;
        StartCoroutine(Wait(duration));
    }

    private IEnumerator Wait(float duration)
    {
        waiting = true;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        waiting = false;
    }
}