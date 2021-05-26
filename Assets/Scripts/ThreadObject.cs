using UnityEngine;

public class ThreadObject : MonoBehaviour
{
    public Transform targetFrom;
    public string tagFrom;
    public Transform targetTo;
    public string tagTo;

    public GameObject ThreadNote;

    void Update()
    {
        if (targetFrom == null || targetTo == null || targetFrom == targetTo)
        {
            Debug.LogError($"Thread missed target from {targetFrom} or target to {targetTo}");
            return;
        }

        transform.position = (targetFrom.position + targetTo.position) * 0.5f;
        var heading = targetFrom.position - targetTo.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, heading);
        Vector3 scale = transform.localScale;
        scale.z = (targetFrom.position - targetTo.position).magnitude;
        transform.localScale = scale;
    }
}
