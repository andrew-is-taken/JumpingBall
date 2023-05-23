using UnityEngine;

[RequireComponent(typeof(MovementManager))]
public class CameraController : MonoBehaviour
{
    private MovementManager movementManager; // movement manager
    private MovementManager.MovementController controller; // movement controller
    private Transform mainCamera; // main camera

    #region Rotation
    private float t; // time for lerp of camera on rotation element
    private bool movingCamera; // camera
    private Vector3 mainCameraOldPos; // start of lerp on rotation element
    private float mainMovementCoordinate; // static coordinate for moving camera
    private float mainMovementDirection; // direction of player movement
    #endregion

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        controller = movementManager.Movement;
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        UpdateCamera(); // updating camera position
    }

    /// <summary>
    /// Updates camera position based on player position.
    /// </summary>
    private void UpdateCamera()
    {
        if (!movingCamera) // main camera normal positioning
        {
            if (movementManager.movingHorizontally)
            {
                mainCamera.position = new Vector3(transform.position.x + 1.5f * mainMovementDirection, mainMovementCoordinate, -10);
            }
            else
            {
                mainCamera.position = new Vector3(mainMovementCoordinate, transform.position.y + 3f * mainMovementDirection, -10);
            }
        }
        else // camera positioning when rotating in corner
        {
            t += Time.deltaTime * (controller.speed / 4); // time for lerp
            Vector3 target; // desired position for camera
            if (movementManager.movingHorizontally)
            {
                target = new Vector3(transform.position.x + 1.5f * mainMovementDirection, mainMovementCoordinate, -10);
                mainCamera.position = Vector3.Lerp(mainCameraOldPos, target, t);
            }
            else
            {
                target = new Vector3(mainMovementCoordinate, transform.position.y + 3f * mainMovementDirection, -10);
                mainCamera.position = Vector3.Lerp(mainCameraOldPos, target, t);
            }
            if (t > 1f) // if camera is in right position
            {
                movingCamera = false;
            }
        }
    }

    /// <summary>
    /// Sets the main properties for camera movement.
    /// </summary>
    /// <param name="new_mainMovementDirection"></param>
    public void SetCoordinate(float new_mainMovementCoordinate)
    {
        movingCamera = true; // start rotation of the camera
        t = 0f; // reset timer for camera lerp
        mainCameraOldPos = mainCamera.position; // start position of camera in rotation
        mainMovementCoordinate = new_mainMovementCoordinate; // update coordinate for camera
    }

    /// <summary>
    /// Sets the new coordinate.
    /// </summary>
    /// <param name="new_mainMovementCoordinate"></param>
    public void SetMainMovementCoordinate(float new_mainMovementCoordinate)
    {
        mainMovementCoordinate = new_mainMovementCoordinate;
    }

    /// <summary>
    /// Sets the new direction.
    /// </summary>
    /// <param name="new_mainMovementDirection"></param>
    public void SetMainMovementDirection(float new_mainMovementDirection)
    {
        mainMovementDirection = new_mainMovementDirection;
    }
}
