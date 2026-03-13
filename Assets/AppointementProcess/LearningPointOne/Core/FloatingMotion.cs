using UnityEngine;

public class FloatingMotion : MonoBehaviour
{
    public float bobSpeed = 1.5f;
    public float bobHeight = 0.2f;
    public float driftSpeed = 0.2f;
    public float rotationSpeed = 30f; 
    private Vector3 _startPos;
    private Vector3 _driftDir;

    private void Start()
    {
        _startPos = transform.position;
        _driftDir = Random.insideUnitSphere;
        _driftDir.y = 0f; // keep drift mostly horizontal
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        float d = Mathf.Sin(Time.time * driftSpeed) * 0.5f;
        transform.position = _startPos + new Vector3(_driftDir.x, 0f, _driftDir.z) * d + new Vector3(0f, y, 0f);

        // Spin around the up axis at the configured speed
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

}