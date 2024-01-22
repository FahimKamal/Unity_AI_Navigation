using UnityEngine;

public class RotateToGroundNormal : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
