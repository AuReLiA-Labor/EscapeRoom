using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSetController_updated : MonoBehaviour
{
    [SerializeField] private Transform balloonsParent;
    [SerializeField] private List<GameObject> balloons = new();

    [Header("Wave Settings")]
    [Tooltip("How many balloons visible per wave.")]
    public int batchSize = 5;

    [Tooltip("Seconds each batch stays visible.")]
    public float waveIntervalSeconds = 15f;

    private Coroutine _routine;
    private int _cursor;
    private readonly List<GameObject> _lastBatch = new();

    public System.Action Completed;

    private void Awake()
    {
        if ((balloons == null || balloons.Count == 0) && balloonsParent)
        {
            balloons = new List<GameObject>();
            for (int i = 0; i < balloonsParent.childCount; i++)
                balloons.Add(balloonsParent.GetChild(i).gameObject);
        }

        foreach (var b in balloons)
            if (b) b.SetActive(false);
    }

    public void BeginSet()
    {
        StopSet(true);
        _cursor = 0;
        _routine = StartCoroutine(RunWaves());
    }

    public void StopSet(bool hideAll)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (hideAll)
        {
            foreach (var b in balloons)
                if (b) b.SetActive(false);
            _lastBatch.Clear();
        }
    }

    private IEnumerator RunWaves()
    {
        while (_cursor < balloons.Count)
        {
            foreach (var go in _lastBatch)
                if (go) go.SetActive(false);
            _lastBatch.Clear();

            int count = 0;
            while (_cursor < balloons.Count && count < batchSize)
            {
                var go = balloons[_cursor++];
                if (go)
                {
                    go.SetActive(true);
                    _lastBatch.Add(go);
                }
                count++;
            }

            yield return new WaitForSeconds(waveIntervalSeconds);
        }

        foreach (var go in _lastBatch)
            if (go) go.SetActive(false);
        _lastBatch.Clear();

        _routine = null;
        Completed?.Invoke();
    }
}
