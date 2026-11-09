using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToastView : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Text messageText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayDuration = 2.5f;

    private Coroutine routine;

    private void Awake()
    {
        if (container == null) container = gameObject;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (container != null) container.SetActive(false);
    }

    public void Show(string message)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(DoShow(message));
    }

    private IEnumerator DoShow(string message)
    {
        if (messageText != null) messageText.text = message;
        if (container != null) container.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float t = 0f;
            while (t < 0.25f)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = t / 0.25f;
                yield return null;
            }
            canvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(displayDuration);

            t = 0f;
            while (t < 0.25f)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - (t / 0.25f);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        else
        {
            yield return new WaitForSecondsRealtime(displayDuration);
        }

        if (container != null) container.SetActive(false);
        routine = null;
    }
}
