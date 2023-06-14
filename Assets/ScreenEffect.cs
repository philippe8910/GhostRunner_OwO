using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class ScreenEffect : MonoBehaviour
{
    [SerializeField] private Volume processData;

    public static ScreenEffect instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        processData = GetComponent<Volume>();
    }

    public void LensDistortionFadeIn()
    {
        processData.profile.TryGet<LensDistortion>(out var lensDistortion);

        DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, -1 , 0.2f).onComplete +=
            delegate
            {
                DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, 0, 0.2f);
            };
    }

    public void CameraShake()
    {
        StartCoroutine(StartShake(.2f, 0.7f));
        
        IEnumerator StartShake(float duration, float magnitude)
        {
            Vector3 originalPos = Camera.main.transform.localPosition;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            Camera.main.transform.localPosition = originalPos;
        }
    }
    

    
}
