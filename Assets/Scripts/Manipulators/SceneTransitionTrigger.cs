using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads a new scene when an object tagged "Player" enters this object's trigger collider.
/// Works with both 3D (Collider) and 2D (Collider2D) physics.
/// </summary>
public class SceneTransitionTrigger : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load. It must be added to the Build Settings / Build Profiles scene list.")]
    [SerializeField] private string sceneToLoad;

    [Tooltip("Tag the player GameObject uses.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Load the scene in the background instead of freezing the game while it loads.")]
    [SerializeField] private bool loadAsync = false;

    private bool hasTriggered;

    // Runs when the script is first added in the Editor: makes the collider a trigger automatically.
    private void Reset()
    {
        if (TryGetComponent(out Collider col3D)) col3D.isTrigger = true;
        if (TryGetComponent(out Collider2D col2D)) col2D.isTrigger = true;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"[{name}] No scene name set on SceneTransitionTrigger.", this);
        }
        else if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError($"[{name}] Scene '{sceneToLoad}' is not in the Build Settings scene list " +
                           "or the name is misspelled.", this);
        }
    }

    // 3D physics
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.CompareTag(playerTag) ||
                        (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(playerTag));
        if (isPlayer) LoadScene();
    }

    // 2D physics
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isPlayer = other.CompareTag(playerTag) ||
                        (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(playerTag));
        if (isPlayer) LoadScene();
    }

    private void LoadScene()
    {
        if (hasTriggered) return;   // prevent loading twice if multiple colliders enter
        hasTriggered = true;

        if (loadAsync)
            SceneManager.LoadSceneAsync(sceneToLoad);
        else
            SceneManager.LoadScene(sceneToLoad);
    }
}
