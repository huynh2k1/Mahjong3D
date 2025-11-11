using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl I;
    [Header("Target")]
    public Transform target;

    [Header("Rotation")]
    public float rotateSpeed = 3f;
    public float minYAngle = 10f;    // Giới hạn nhìn lên
    public float maxYAngle = 80f;    // Giới hạn nhìn xuống

    private float rotX;
    private float rotY;

    [Header("Zoom")]
    public float distanceDefault = 5f;
    public float zoomSpeed = 1f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    private float distanceCurrent;

    private Vector3 lastMousePos;
    private Quaternion initRotation;
    private float initRotX;
    private float initRotY;


    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        distanceCurrent = distanceDefault;

        if (target == null) return;

        Vector3 angles = transform.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;

        // ✅ Lưu lại góc ban đầu
        initRotation = transform.rotation;
        initRotX = rotX;
        initRotY = rotY;
    }

    // ✅ Reset góc xoay lẫn vị trí
    public void ResetCamera()
    {
        // Reset góc xoay
        rotX = initRotX;
        rotY = initRotY;

        // Reset khoảng cách
        distanceCurrent = distanceDefault;

        // Cập nhật lại vị trí
        UpdateCameraPosition();
    }

    void Update()
    {
        if (GameManager.I.CurState != GameState.Play)
            return;

        if (target == null)
        {
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleRotationPC();
        HandleZoomPC();
#else
        HandleRotationMobile();
        HandleZoomMobile();
#endif

        ClampRotation();
        UpdateCameraPosition();
    }

    // ✅ PC: Drag xoay
    void HandleRotationPC()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            rotX += delta.x * rotateSpeed * Time.deltaTime;
            rotY -= delta.y * rotateSpeed * Time.deltaTime;
        }
    }

    // ✅ PC: Scroll zoom
    void HandleZoomPC()
    {
        float s = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(s) > 0.01f)
        {
            distanceCurrent -= s * zoomSpeed;
            distanceCurrent = Mathf.Clamp(distanceCurrent, minDistance, maxDistance);
        }
    }

    // ✅ Mobile: 1 ngón xoay
    void HandleRotationMobile()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                rotX += t.deltaPosition.x * rotateSpeed * Time.deltaTime;
                rotY -= t.deltaPosition.y * rotateSpeed * Time.deltaTime;
            }
        }
    }

    // ✅ Mobile: Pinch zoom
    void HandleZoomMobile()
    {
        if (Input.touchCount == 2)
        {
            Touch a = Input.GetTouch(0);
            Touch b = Input.GetTouch(1);

            float prev = (a.position - a.deltaPosition - (b.position - b.deltaPosition)).magnitude;
            float curr = (a.position - b.position).magnitude;
            float diff = curr - prev;

            distanceCurrent -= diff * zoomSpeed * Time.deltaTime;
            distanceCurrent = Mathf.Clamp(distanceCurrent, minDistance, maxDistance);
        }
    }

    // ✅ Giới hạn góc nhìn lên / xuống
    void ClampRotation()
    {
        rotY = Mathf.Clamp(rotY, minYAngle, maxYAngle);
    }

    // ✅ Cập nhật camera
    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 dir = rotation * Vector3.back;

        transform.position = target.position + dir * distanceCurrent;
        transform.LookAt(target);
    }

    // ✅ Reset về khoảng cách mặc định
    public void ResetCameraDistance()
    {
        distanceCurrent = distanceDefault;
    }
}
