using UnityEngine;
using System;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    #region Main
    public static MovementManager instance { get; private set; } // this only instance
    public MovementController Movement; // controller of player
    private CameraController cameraController; // controller of camera
    private LevelManager levelManager; // manager
    private Rigidbody2D m_Rigidbody; // player's rigidbody

    [field: SerializeField] public bool rotating { get; private set; } // if player is on a rotation element
    [field: SerializeField] public GameObject DeathParticles { get; private set; } // death effect
    #endregion

    #region Hints
    [SerializeField] private Transform HintVectorPrefab;// prefab of vector that indicates direction of next jump
    [SerializeField] private Transform HintDangerPrefab; // prefab of sign that indicates an obstacle nearby
    [SerializeField] private Transform ProjectionPrefab; // prefab of result of next jump

    private Transform HintVector; // vector that indicates direction of next jump
    private Transform HintDanger; // sign that indicates an obstacle nearby
    private Transform Projection; // result of next jump
    private bool renderingVector; // if rendering vector
    private bool renderingDanger = false; // if rendering sign
    private Vector3 hintVectorPos; // position of hint vector
    #endregion

    #region Finish
    public bool movingHorizontally { get; private set; } // if player is moving horizontally
    public float endLevelBonus { get; private set; } // bonus at the end of level

    private bool finished; // if player crossed the finish line
    private bool gameEnded; // if the game has ended
    private bool startOfLevel; // if user started the game
    #endregion

    #region Last checkpoint
    private Rotation lastCheckpoint; // the last rotation element with chechpoint
    private Vector2 lastCheckpointPos; // position of last checkpoint
    //private Vector2 lastCheckpointMainDir; // main direction on last checkpoint
    //private Vector2 lastCheckpointAddDir; // additional direction on last checkpoint
    //private float lastCheckpointSpeed; // speed on last checkpoint
    private bool lastCheckpointWasStart; // speed on last checkpoint
    private bool passedOnlyStart; // to solve the problem when player jumps in rotation from wrong side and crosses only start of rotation
    #endregion

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 movement managers in the scene");
        }
        instance = this;

        m_Rigidbody = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        cameraController = GetComponent<CameraController>();

        startOfLevel = true; // indicates that user didn't tap on screen for the first time
        lastCheckpointWasStart = true;
        finished = false; // indicates that player didn't finish
        endLevelBonus = 1f; // default bonus

        InstantiateHints();

        Movement.mainDirection = Movement.startDirection;
        movingHorizontally = Movement.mainDirection.y == 0; // whether player starts to move horizontally
        SetInitialCameraCoordinateAndDirection(movingHorizontally ? transform.position.y : transform.position.x);
    }

    private void InstantiateHints()
    {
        HintVector = HintVector == null ? Instantiate(HintVectorPrefab) : HintVector;
        HintVector.gameObject.SetActive(false);

        HintDanger = HintDanger == null ? Instantiate(HintDangerPrefab) : HintDanger;
        HintDanger.gameObject.SetActive(false);

        Projection = Projection == null ? Instantiate(ProjectionPrefab) : Projection;
        Projection.gameObject.SetActive(false);
    }

    /// <summary>
    /// Real start of the level.
    /// Called when user touches the screen to start the level.
    /// </summary>
    public void StartFromTap()
    {
        GetComponentInChildren<TrailRenderer>().emitting = true;
        if (startOfLevel)
        {
            Movement.Start(m_Rigidbody, GetComponent<AudioSource>(), GetComponentsInChildren<AudioSource>()[1]); // initial player setup

            DefineDifficulty(); // set difficulty
            HintVisible(false); // hide hints at the start

            SetCheckpoint(null, transform.position, Movement.startDirection, Movement.startDirection, Movement.speed); // first checkpoint
            startOfLevel = false; // level started
        }
    }

    private void Update()
    {
        UpdateHints(); // hints positioning
    }

    private void LateUpdate()
    {
        CheckIfPlayerFlewAway(); // check if player flew away from the screen
        CheckWallInAdditionalDirection(); // check if we need to throw player in void
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy") // when player collides with enemy
        {
            StartCoroutine(Death());
        }
        else // when player collides with ground
        {
            HintVisible(true);
            Movement.ContinueAudio();
            if (Movement.waitingToChangeDirection) // if player collides after rotation
            {
                Movement.SetMovementDirectionDelayed(passedOnlyStart); // setting new direction of movement
                hintVectorPos = Movement.hintVectorPositionAndRot(HintVector); // calculate position of vector
            }
            else
            {
                if (!rotating)
                {
                    Movement.canJump = true; //  player can jump if not in the rotation element
                    Movement.PlayLandingSound();
                }
            }
            AddRotation(); // spinning player
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!rotating)
            Movement.ResetSpeed(); // recalculate speed if not in rotation element
        else
            Movement.ResetSpeedOnRotation(); // recalculate speed if in rotation element
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    //if (finished && mMovement.canJump)
    //    //{
    //    //    //print("HERE");
    //    //    //mMovement.AddForceInAdditionalDirection();
    //    //}
    //    //ClearRotation(); // stop player rotation after jump
    //}

    /// <summary>
    /// Called when user touches the screen to jump.
    /// </summary>
    public void TapOnScreen()
    {
        Movement.ChangeDirection(); // player jumps
        ClearRotation();
        hintVectorPos = Movement.hintVectorPositionAndRot(HintVector); // updating vector
        HintVisible(false); // disabling hints
    }

    /// <summary>
    /// Positioning hints.
    /// </summary>
    private void UpdateHints()
    {
        if (rotating) // if in rotation element
        {
            HintVisible(false);
        }
        else
        {
            if (Movement.canJump) // if player is grounded
            {
                if (renderingVector) // if we need to render vector
                {
                    HintVector.position = transform.position + hintVectorPos; // position of vector based on player position and offset

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, hintVectorPos, 10); // raycast to place the projection

                    if (hit)
                    {
                        HintVisible(true);
                        Projection.transform.position = hit.point - new Vector2(hintVectorPos.x, hintVectorPos.y).normalized * Mathf.Sqrt(2) * .2f; // position of projection
                        if (hit.distance < 1.4f)
                            HintVector.gameObject.SetActive(false);
                    }
                    else
                    {
                        HintVisible(false);
                    }
                }
                if (renderingDanger) // if we need to render danger sign
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, m_Rigidbody.velocity, 8); // raycast to check for danger

                    if (hit.collider == null) // if there is no collision at all
                    {
                        HintDanger.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (hit.collider.tag == "Enemy") // if there is a danger
                        {
                            HintDanger.gameObject.SetActive(true);
                            if (movingHorizontally)
                                HintDanger.position = transform.position + new Vector3(0, .5f, 0); // positioning sign
                            else
                                HintDanger.position = transform.position + new Vector3(0, .5f * Movement.mainDirection.y, 0); // positioning sign
                        }
                        else // if there is no danger
                        {
                            HintDanger.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if we need to throw player in void.
    /// </summary>
    private void CheckWallInAdditionalDirection()
    {
        if (!Movement.CheckWallInAdditionalDirection(transform.position))
            HintVisible(false);
    }

    /// <summary>
    /// Checks if player flew away from the screen.
    /// </summary>
    private void CheckIfPlayerFlewAway()
    {
        Vector3 pos = transform.position; // player position

        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)); // top right corner of the screen
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)); // bottom left corner of the screen

        if (!gameEnded && (pos.x > topRight.x || pos.x < bottomLeft.x || pos.y > topRight.y || pos.y < bottomLeft.y)) // if player is outsideof the screen
        {
            gameEnded = true;
            if (finished) // if player crossed the finish line
                Finish();
            else
                StartCoroutine(Death());
        }
    }

    /// <summary>
    /// Death of player.
    /// </summary>
    private IEnumerator Death()
    {
        yield return new WaitForEndOfFrame();
        GetComponentInChildren<TrailRenderer>().emitting = false;
        GameObject fx = Instantiate(DeathParticles, transform.position, Quaternion.Euler(0, -90, 0)); // spawn the effect
        fx.GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume; // set the volume
        Destroy(fx, 2.5f); // timer to destroy effect
        gameObject.SetActive(false); // remove player
        levelManager.Death(); // death event
    }

    /// <summary>
    /// Player finished the level.
    /// </summary>
    public void Finish()
    {
        HintVisible(false);
        levelManager.Finish(endLevelBonus); // finish event
    }

    /// <summary>
    /// Sets the difficulty of the level.
    /// </summary>
    private void DefineDifficulty()
    {
        hintVectorPos = Movement.hintVectorPositionAndRot(HintVector); // initial position of vector
        switch (levelManager.difficulty)
        {
            case 0: // if easy difficulty
                HintVector.gameObject.SetActive(true);
                HintDanger.gameObject.SetActive(true);
                renderingVector = true;
                renderingDanger = true;
                break;
            case 1: // if medium difficulty
                HintDanger.gameObject.SetActive(true);
                renderingDanger = true;
                break;
        }
    }

    /// <summary>
    /// <para>Changes the main direction when player ENTERS the rotation on level.</para>
    /// Changes the additional direction if player didn't jump before the rotation on level, <br/>
    /// otherwise the additional direction will be calculated later when player collides with level border.
    /// <para>Called from OnTriggerEnter of rotation element.</para>
    /// </summary>
    /// <param name="newDirection"></param>
    /// <param name="newMainCoord"></param>
    /// <param name="inverted"></param>
    public void SetMovementDirection(Vector3 newDirection, float newMainCoord, bool inverted)
    {
        passedOnlyStart = true; // set to true to check if player collides with ground after start of rotation
        rotating = true; // player is in rotation element

        Movement.SetMovementDirection(newDirection, inverted); // change player direction
        cameraController.SetCoordinate(newMainCoord);

        movingHorizontally = Movement.GetMovingHorizontally(); // update if player moves horizontally after rotation

        HintVisible(false);

        if (movingHorizontally)
        {
            cameraController.SetMainMovementDirection(newDirection.x); // update direction for camera
        }
        else
        {
            cameraController.SetMainMovementDirection(newDirection.y); // update direction for camera
        }
    }

    /// <summary>
    /// Pushes player in direction after rotation element.
    /// </summary>
    /// <param name="newDirection"></param>
    public void AddForceInDirection(Vector3 newDirection)
    {
        passedOnlyStart = false;
        rotating = false;

        Movement.AddMainForceInDirection(newDirection); // push player
        hintVectorPos = Movement.hintVectorPositionAndRot(HintVector); // update vector position

        AddRotation();
        HintVisible(true);
    }

    /// <summary>
    /// Changes the visibility of hints in the game.
    /// </summary>
    /// <param name="isOn">True - enable hints, False - disable</param>
    private void HintVisible(bool isOn)
    {
        if (renderingVector)
        {
            HintVector.gameObject.SetActive(isOn);
            Projection.gameObject.SetActive(isOn);
        }
        if (renderingDanger)
        {
            HintDanger.gameObject.SetActive(isOn);
        }
    }

    /// <summary>
    /// Sets player's speed.
    /// </summary>
    /// <param name="newSpeed"></param>
    public void SetSpeed(float newSpeed)
    {
        Movement.speed = newSpeed;
        Movement.ResetSpeed();
    }

    /// <summary>
    /// Tells player's speed
    /// </summary>
    /// <returns>Player's speed.</returns>
    public float GetSpeed()
    {
        return Movement.speed;
    }

    /// <summary>
    /// Adds the spin effect to the player based on speed.
    /// </summary>
    public void AddRotation()
    {
        float speed = Movement.speed;
        if (movingHorizontally)
            m_Rigidbody.angularVelocity = speed * Movement.mainDirection.x * (Movement.additionalDirection.y == 1 ? 100 : -100);
        else
            m_Rigidbody.angularVelocity = speed * Movement.mainDirection.y * (Movement.additionalDirection.x == 1 ? -100 : 100);
    }

    /// <summary>
    /// Removes player's spin.
    /// </summary>
    public void ClearRotation()
    {
        m_Rigidbody.angularVelocity = 0;
    }

    /// <summary>
    /// Sets new checpoint where player can respawn.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mainDir"></param>
    /// <param name="additionalDir"></param>
    /// <param name="speed"></param>
    /// <param name="inverted"></param>
    public void SetCheckpoint(Rotation last, Vector2 position, Vector2 mainDir, Vector2 addDir, float speed)
    {
        if (!startOfLevel)
            lastCheckpointWasStart = false;
        lastCheckpoint = last;
        lastCheckpointPos = position;
        //lastCheckpointMainDir = mainDir;
        //lastCheckpointAddDir = addDir;
        //lastCheckpointSpeed = speed;
    }

    /// <summary>
    /// Respawns player on last checkpoint.
    /// </summary>
    public void RespawnOnLastCheckpoint()
    {
        if (!lastCheckpointWasStart)
        {
            gameObject.SetActive(true);
            gameEnded = false;
            GetComponentInChildren<TrailRenderer>().emitting = false;
            levelManager.PrepareMainUI();

            transform.position = lastCheckpointPos;
            Movement.mainDirection = lastCheckpoint.newDirection;
            Movement.additionalDirection = Vector3.zero;
            //Movement.additionalDirection = lastCheckpointAddDir;
            Movement.speed = lastCheckpoint.newSpeed;
            Movement.SetMovingHorizontally();
            if (movingHorizontally)
                Movement.additionalDirection.y = lastCheckpointPos.y >= lastCheckpoint.transform.position.y ? 1 : -1;
            else
                Movement.additionalDirection.x = lastCheckpointPos.x >= lastCheckpoint.transform.position.x ? 1 : -1;
            //Movement.RecalculateAdditionalDirectionOnRotation(false);
            lastCheckpoint.TurnOnFollowingPart();
            //transform.position = lastCheckpointPos;
            //Movement.mainDirection = lastCheckpointMainDir;
            //Movement.additionalDirection = lastCheckpointAddDir;
            //Movement.speed = lastCheckpointSpeed;
            //Movement.SetMovingHorizontally();
            //Movement.RecalculateAdditionalDirectionOnRotation(false);
            hintVectorPos = Movement.hintVectorPositionAndRot(HintVector); // calculate position of vector

            movingHorizontally = Movement.movingHorizontally; // whether player starts to move horizontally
            SetInitialCameraCoordinateAndDirection(movingHorizontally ? lastCheckpoint.transform.position.y : lastCheckpoint.transform.position.x);

            Movement.AddMainForceInDirection(Movement.mainDirection);

            foreach (GhostEnemy enemy in FindObjectsOfType<GhostEnemy>())
            {
                enemy.RestartEnemy();
            }

            cameraController.UpdateCamera();
        }
        else
        {
            levelManager.RestartLevel(false);
        }
    }

    /// <summary>
    /// Resets the coordinate and direction of camera controller.
    /// </summary>
    private void SetInitialCameraCoordinateAndDirection(float coordinate)
    {
        cameraController.SetMainMovementDirection(movingHorizontally ? Movement.mainDirection.x : Movement.mainDirection.y); // main direction at the start
        cameraController.SetMainMovementCoordinate(coordinate); // main coordinate at the start
    }

    /// <summary>
    /// When player crossed finish line but did not finished the bonus collection.
    /// </summary>
    public void CrossedFinishLine()
    {
        finished = true;
    }

    /// <summary>
    /// Sets the bonus.
    /// </summary>
    /// <param name="bonus"></param>
    public void SetEndLevelBonus(float bonus)
    {
        endLevelBonus = bonus;
    }
}