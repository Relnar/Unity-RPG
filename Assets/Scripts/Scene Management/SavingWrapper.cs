using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{

    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        [SerializeField]
        float fadeInTime = 1.0f;

        SavingSystem savingSystem;

        private void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return savingSystem.LoadLastScene(defaultSaveFile);

            Fader fader = FindObjectOfType<Fader>();
            if (fader)
            {
                fader.FadeOutImmediate();
            }

            yield return fader.FadeIn(fadeInTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            print("Load");
            savingSystem.Load(defaultSaveFile);
        }

        public void Save()
        {
            print("Save");
            savingSystem.Save(defaultSaveFile);
        }

        public void Delete()
        {
            print("Delete");
            savingSystem.Delete(defaultSaveFile);
        }
    }
}
