using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Balloon_updated : MonoBehaviour, IInteractable
{
    [Header("Fact Settings")]
    public bool isCorrect;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem correctVfx;
    [SerializeField] private ParticleSystem wrongVfx;
    [SerializeField] private AudioClip ding;
    [SerializeField] private AudioClip buzz;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.9f;

    private bool _popped;

    public void Interact(Vector3 hitPoint)
    {
        if (_popped) return;
        _popped = true;

        if (GameManager_updated.Instance)
            GameManager_updated.Instance.ProcessBalloonPop(isCorrect);

        if (isCorrect && correctVfx) Instantiate(correctVfx, transform.position, Quaternion.identity);
        if (!isCorrect && wrongVfx) Instantiate(wrongVfx, transform.position, Quaternion.identity);

        var clip = isCorrect ? ding : buzz;
        if (clip) AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolume);

        Destroy(gameObject);
    }
}