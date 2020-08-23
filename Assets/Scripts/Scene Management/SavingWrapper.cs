using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{

    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        SavingSystem savingSystem;

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            savingSystem = GetComponent<SavingSystem>();
            yield return savingSystem.LoadLastScene(defaultSaveFile);

            yield return fader.FadeIn(1.0f);
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
    }
}
