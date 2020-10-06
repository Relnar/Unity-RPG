using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            nothing,
            townEntrance,
            townExit,
            corridorEntrance,
            corridorExit
        }

        [SerializeField]
        int sceneToLoad = -1;

        [SerializeField]
        Transform spawnPoint = null;

        [SerializeField]
        DestinationIdentifier id = DestinationIdentifier.nothing;

        [SerializeField]
        DestinationIdentifier destinationId = DestinationIdentifier.nothing;

        [Range(0.000001f, Mathf.Infinity)]
        [SerializeField]
        float fadeOutTime = 1.0f;

        [Range(0.000001f, Mathf.Infinity)]
        [SerializeField]
        float fadeInTime = 1.5f;

        [SerializeField]
        float fadeWaitTime = 0.5f;

        Fader fader;

        private void Start()
        {
            if (sceneToLoad < 0)
            {
                enabled = false;
                print("Error: sceneToLoad is invalid!");
            }
            fader = FindObjectOfType<Fader>();
            if (fader == null)
            {
                print("Fader not found");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the player enter the portal, start the transition to the new scene
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            // Player from the previous scene
            PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            player.enabled = false;

            // Start fading out the new scene
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            // Load asynchronously the new scene
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // Player from the new scene
            PlayerController newPlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayer.enabled = false;

            savingWrapper.Load();

            // Find the destination portal
            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayer(destinationPortal);

            savingWrapper.Save();

            // Keep the new scene faded out for some time
            yield return new WaitForSeconds(fadeWaitTime);

            // Start fading in the new scene
            fader.FadeIn(fadeInTime);

            // Restore control to the new player
            newPlayer.enabled = true;

            // Destroy the portal
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            if (otherPortal)
            {
                GameObject player = GameObject.FindWithTag("Player");

                // Warp to the spawn point to avoid conflicts between the navmesh agent and
                // the player position if changed manually
                player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.transform.position);

                // Turn the player in the portal's z direction
                player.transform.rotation = otherPortal.transform.rotation;
            }
        }

        private Portal GetDestinationPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this ||
                    portal.id != destinationId)
                {
                    continue;
                }
                return portal;
            }

            return null;
        }
    }
}

