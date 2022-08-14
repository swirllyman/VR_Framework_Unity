using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class LoadingSphere : MonoBehaviour
{
    public delegate void OnSphereFinished(bool faded);
    public static event OnSphereFinished onSphereFinished;

    public bool startFaded = true;
    public bool useScreensaver = false;
    public GameObject screenSaverObject;

    public Image[] screenSaverImages;

    [SerializeField] Renderer sphereRend;
    [SerializeField] Renderer logoRend;
    float currentFillPerc = 1.0f;
    int fadeDir = 0;

    const string SPHERE_FILL_SHADER = "_FillAmount";
    Coroutine currentFadeRoutine;

    private void Awake()
    {
        sphereRend.enabled = true;
        logoRend.enabled = true;
        screenSaverObject.SetActive(true && useScreensaver);
        if (startFaded)
        {
            FadeIn(0.0f);
        }
    }

    public void FadeIn(float newFadeTime)
    {
        fadeDir = 1;
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(ToggleFade(newFadeTime));
    }

    public void FadeOut(float waitTime, float newFadeTime)
    {
        fadeDir = -1;
        StartCoroutine(FadeAfterTime(waitTime, newFadeTime));
    }

    IEnumerator FadeAfterTime(float timeToWait, float newFadeTime)
    {
        yield return new WaitForSeconds(timeToWait);
        logoRend.enabled = false;
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(ToggleFade(newFadeTime));
    }

    IEnumerator ToggleFade(float newFadeTime)
    {
        for (float i = 0; i < newFadeTime; i += Time.deltaTime)
        {
            currentFillPerc += fadeDir * (Time.deltaTime / newFadeTime);
            currentFillPerc = Mathf.Clamp01(currentFillPerc);
            sphereRend.material.SetFloat(SPHERE_FILL_SHADER, currentFillPerc);

            for (int j = 0; j < screenSaverImages.Length; j++)
            {
                screenSaverImages[j].color = new Color(screenSaverImages[j].color.r, screenSaverImages[j].color.g, screenSaverImages[j].color.b, currentFillPerc);
            }
            yield return null;
        }

        if(fadeDir < 0)
        {
            currentFillPerc = 0.0f;
            onSphereFinished?.Invoke(true);
        }
        else
        {
            currentFillPerc = 1.0f;
            logoRend.enabled = true;
            onSphereFinished?.Invoke(false);
        }

        for (int j = 0; j < screenSaverImages.Length; j++)
        {
            screenSaverImages[j].color = new Color(screenSaverImages[j].color.r, screenSaverImages[j].color.g, screenSaverImages[j].color.b, currentFillPerc);
        }

        sphereRend.material.SetFloat(SPHERE_FILL_SHADER, currentFillPerc);
    }
}
