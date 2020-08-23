using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Nesting co-routines
/*
        IEnumerator FadeOutIn()
        {
            yield return FadeOut(fadeTime);
            yield return FadeIn(fadeTime);
        }
*/
        public IEnumerator FadeIn(float fadeTime)
        {
            while (canvasGroup.alpha > 0.0f)
            {
                canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha - Time.deltaTime / fadeTime);
                yield return null;
            }
        }

        public IEnumerator FadeOut(float fadeTime)
        {
            while (canvasGroup.alpha < 1.0f)
            {
                canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha + Time.deltaTime / fadeTime);
                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1.0f;
        }
    }
}
