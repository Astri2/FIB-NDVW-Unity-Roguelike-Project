using System.Collections;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Watches the scene for the presence (or replacement) of a GameObject named "Generated Level"
/// and triggers an A* scan when it's created/changed. This avoids modifying Edgar's code.
/// Attach this component to any persistent GameObject (e.g. GameManager).
/// </summary>
public class AstarRescanWatcher : MonoBehaviour
{
    [Tooltip("Name of the generated level root object created by Edgar")]
    public string generatedLevelName = "Generated Level";

    [Tooltip("How often (seconds) to poll for the generated level")]
    public float pollInterval = 0.25f;

    // Instance id of last seen generated level root. When it changes, we rescan.
    private int lastSeenInstanceId = 0;

    private Coroutine watcherCoroutine;

    private void OnEnable()
    {
        watcherCoroutine = StartCoroutine(WatchForGeneratedLevel());
    }

    private void OnDisable()
    {
        if (watcherCoroutine != null)
            StopCoroutine(watcherCoroutine);
    }

    private IEnumerator WatchForGeneratedLevel()
    {
        while (true)
        {
            var root = GameObject.Find("/Generated Level");

            if (root != null)
            {
                int id = root.GetInstanceID();

                if (id != lastSeenInstanceId)
                {
                    lastSeenInstanceId = id;
                    RescanAstar();

                    // One-shot behaviour: stop watching after first successful rescan
                    yield break;
                }
            }

            yield return new WaitForSeconds(pollInterval);
        }
    }

    private void RescanAstar()
    {
        if (AstarPath.active == null)
        {
            Debug.LogWarning("A* object (AstarPath) not found in scene — cannot rescan.");
            return;
        }

        Debug.Log("Detected Generated Level -> rescanning A* pathfinding");
        AstarPath.active.Scan();
    }
}
