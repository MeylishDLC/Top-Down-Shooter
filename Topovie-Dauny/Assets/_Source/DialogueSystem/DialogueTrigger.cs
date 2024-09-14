using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Visual Cue")] 
        [SerializeField] private SpriteRenderer visualCue;

        [Header("Ink JSON")] 
        [SerializeField] private TextAsset inkJSON;
        
        private bool playerInRange;
        private void Awake()
        {
            visualCue.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (playerInRange)
            {
                visualCue.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //todo play dialogue
                }
            }
            else
            {
                visualCue.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerInRange = false;
            }
        }
    }
}
