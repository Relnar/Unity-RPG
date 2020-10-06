using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator Fade(float target, float fadeTime)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, fadeTime));
            yield return currentActiveFade;
        }

        public IEnumerator FadeIn(float fadeTime)
        {
            return Fade(0.0f, fadeTime);
        }

        public IEnumerator FadeOut(float fadeTime)
        {
            return Fade(1.0f, fadeTime);
        }

        public void FadeOutImmediate()
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            canvasGroup.alpha = 1.0f;
        }

        IEnumerator FadeRoutine(float target, float fadeTime)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / fadeTime);
                yield return null;
            }
        }
    }
}
