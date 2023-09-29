using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public Transform charCamera;
    public float camSensitivity = 1;
    public float camSmoothing = 2;

    Vector2 currentMouseLook;
    Vector2 appliedMouseDelta;

    void Start()
    {
        LockCursor(true);
    }

    public void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void Update()
    {
        Look();
    }

    void Look()
    {
        Vector2 smoothMouseDelta = Vector2.Scale(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")), Vector2.one * camSensitivity * camSmoothing);
        appliedMouseDelta = Vector2.Lerp(appliedMouseDelta, smoothMouseDelta, 1 / camSmoothing);
        currentMouseLook += appliedMouseDelta;
        currentMouseLook.y = Mathf.Clamp(currentMouseLook.y, -90, 90);

        charCamera.localRotation = Quaternion.AngleAxis(-currentMouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(currentMouseLook.x, Vector3.up);
    }
}
