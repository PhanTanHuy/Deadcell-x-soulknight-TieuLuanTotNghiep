using System.Collections;
using UnityEngine;

/// <summary>
/// Camera shake đơn giản cho 2D.
/// - Gắn script này lên Camera (hoặc 1 GameObject cha chứa camera).
/// - Gọi CameraShake.Instance.Shake(0.5f, 0.3f) để rung.
/// </summary>
public class ShakeCam : MonoBehaviour
{
    public static ShakeCam Instance;

    [Header("Thiết lập mặc định")]
    [Tooltip("Thời gian rung (giây)")]
    public float defaultDuration = 0.4f;
    [Tooltip("Biên độ rung (độ dịch chuyển tối đa)")]
    public float defaultMagnitude = 0.3f;
    [Tooltip("Nếu true thì dùng Perlin noise cho chuyển động mượt hơn")]
    public bool usePerlin = true;
    [Tooltip("Tốc độ cho Perlin noise")]
    public float perlinSpeed = 20f;

    Vector3 originalPos;
    Coroutine shakeRoutine;
    private float lastMagnitude;
    void Awake()
    {
        // Singleton đơn giản
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        originalPos = transform.localPosition;
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void OnDisable()
    {
        // reset vị trí khi component bị disable
        transform.localPosition = originalPos;
    }

    /// <summary>
    /// Gọi để bắt đầu rung với mặc định.
    /// </summary>
    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    /// <summary>
    /// Gọi để bắt đầu rung với thời gian và biên độ cụ thể.
    /// </summary>
    /// <param name="duration">Thời gian rung (giây)</param>
    /// <param name="magnitude">Biên độ (độ dịch chuyển tối đa)</param>
    public void Shake(float duration = 0.2f, float magnitude = 0.2f)
    {
        if (magnitude < lastMagnitude) return;
        lastMagnitude = magnitude;
        // nếu đang rung thì dừng rồi bắt đầu lại (có thể thay đổi tuỳ ý)
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    /// <summary>
    /// Dừng rung lập tức và đặt về vị trí ban đầu.
    /// </summary>
    public void StopShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }
        transform.localPosition = originalPos;
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        // dùng để offset Perlin để tránh lặp pattern
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // dùng unscaled để không bị ảnh hưởng bởi timeScale
            float percentComplete = elapsed / duration;
            // giảm dần biên độ (ease out)
            float damper = 1.0f - Mathf.Clamp01(percentComplete);

            Vector3 offset;
            if (usePerlin)
            {
                // Perlin noise mượt
                float nx = (Mathf.PerlinNoise(seedX, Time.unscaledTime * perlinSpeed) - 0.5f) * 2f;
                float ny = (Mathf.PerlinNoise(seedY, Time.unscaledTime * perlinSpeed) - 0.5f) * 2f;
                offset = new Vector3(nx, ny, 0f) * magnitude * damper;
            }
            else
            {
                // random tức thời
                float rx = Random.Range(-1f, 1f);
                float ry = Random.Range(-1f, 1f);
                offset = new Vector3(rx, ry, 0f) * magnitude * damper;
            }

            transform.localPosition = originalPos + offset;
            yield return null;
        }

        // kết thúc: đặt lại vị trí
        transform.localPosition = originalPos;
        shakeRoutine = null;
        lastMagnitude = 0f;
    }
    //-------------------- Time Stop --------------------//
    bool waiting = false;
    public void Stop(float duration, float timeScale)
    {
        if (duration <= 0 || timeScale <= 0)
            return;
        if (waiting)
            return;
        Time.timeScale = timeScale;
        StartCoroutine(Wait(duration));
    }
    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}
