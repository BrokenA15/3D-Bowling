using UnityEngine;
using System.Collections;

public class PanelFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeTime = 0.3f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // 🔥 HABILITAR INPUT AL MOSTRAR
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        StartCoroutine(Fade(1));
    }

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return; // ya está oculto

        // 🔥 DESHABILITAR INPUT DURANTE EL HIDE
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = target;

        // Si se ocultó, lo desactivamos
        if (target == 0)
            gameObject.SetActive(false);
    }
}
