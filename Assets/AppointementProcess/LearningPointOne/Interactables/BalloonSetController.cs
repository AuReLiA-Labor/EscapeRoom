using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSetController : MonoBehaviour
{
    [Header("Populate either parent or explicit list")]
    [SerializeField] private Transform balloonsParent;
    [SerializeField] private List<GameObject> balloons = new List<GameObject>();

    [Header("Wave settings")]
    public int batchSize = 5;             // 5 balloons at a time
    public float waveIntervalSeconds = 17; // visible time per batch

    public System.Action Completed;

    int _cursor;
    Coroutine _runRoutine;
    readonly List<GameObject> _lastBatch = new List<GameObject>();

    void Awake()
    {
        if (balloons.Count == 0 && balloonsParent)
        {
            balloons.Clear();
            for (int i = 0; i < balloonsParent.childCount; i++)
            {
                balloons.Add(balloonsParent.GetChild(i).gameObject);
            }
        }

        // Ensure all are hidden initially
        foreach (var b in balloons) if (b) b.SetActive(false);
    }

    public void BeginSet()
    {
        StopSet(true);
        _cursor = 0;
        _runRoutine = StartCoroutine(RunWaves());
    }

    public void StopSet(bool hideAll)
    {
        if (_runRoutine != null)
        {
            StopCoroutine(_runRoutine);
            _runRoutine = null;
        }

        if (hideAll)
        {
            foreach (var b in balloons) if (b) b.SetActive(false);
            _lastBatch.Clear();
        }
    }

    IEnumerator RunWaves()
    {
        while (_cursor < balloons.Count)
        {
            // Hide previous batch
            foreach (var go in _lastBatch) if (go) go.SetActive(false);
            _lastBatch.Clear();

            // Show next batch
            int count = 0;
            while (_cursor < balloons.Count && count < batchSize)
            {
                var go = balloons[_cursor++];
                if (go) { go.SetActive(true); _lastBatch.Add(go); }
                count++;
            }

            yield return new WaitForSeconds(waveIntervalSeconds);
        }

        // Hide the last batch
        foreach (var go in _lastBatch) if (go) go.SetActive(false);
        _lastBatch.Clear();

        _runRoutine = null;
        Completed?.Invoke();
    }
}
