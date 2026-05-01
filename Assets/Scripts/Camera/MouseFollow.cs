using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollow : MonoBehaviour
{
    private void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        transform.position = mouseWorld;
    }
}