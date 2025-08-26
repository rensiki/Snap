using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // Slider로 값을 조절하기 위해 Range를 정해준다.
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;     // 하루의 길이
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;    // 자정의 각도

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;   // 그라데이션
    public AnimationCurve sunIntensity;  // AnimationCurve를 통해 그래프를 생성할 수 있다. 해당 그래프에서 원하는 값들을 Time 값을 통해 꺼내올 수 있다.

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;   // 그라데이션
    public AnimationCurve moonIntensity;  // AnimationCurve를 통해 그래프를 생성할 수 있다. 해당 그래프에서 원하는 값들을 Time 값을 통해 꺼내올 수 있다.

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;      // 풍경광
    public AnimationCurve reflectionIntensityMultiplier;    // 반사광

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        // time을 계속해서 증가시켜주는 데, time을 퍼센테이지로 사용하기 위해 1.0f의 나머지 연산을 해준다.
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        // Sun과 Moon의 빛의 차이가 얼마 없는 것을 수정하는 코드
        // ambientIntensity : 풍경광.
        // reflectionIntensity : 반사광.
        // 시간에 따라 풍경광과 반사광을 변화시켜 낮과 밤의 밝기 차이를 최대화
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
    {
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensityCurve.Evaluate(time);

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy) go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy) go.SetActive(true);
    }
}