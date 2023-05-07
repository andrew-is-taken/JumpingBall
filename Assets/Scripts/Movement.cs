using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    [Serializable]
    public class MainMovement
    {
        public float speed = 8f; // current desired speed of player

        public Vector3 startDirection; // direction of player when level starts
        public Vector3 mainDirection; // main direction of player movement
        public Vector3 additionalDirection = new Vector3(1, 0, 0); // additional direction of player movement

        public bool movingHorizontally; // if player is currently moving horizontally

        public bool canJump; // if player can jump
        public bool waitingToChangeDirection; // if player needs to recalculate directions after rotation when he collides with border

        private Rigidbody2D m_Rigidbody; // rigidbody of player
        private AudioSource m_AudioSource; // audio source of player

        private Vector3 savedDirection; // main direction that will be set after rotation
        private Vector3 offsetForRaycast; // offset for raycasts that check wall to throw player away

        private bool invertedRotation; // is player is in inverted rotation element

        /// <summary>
        /// Initial setup of player.
        /// Called when user touches the screen to start the level.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="audioSource"></param>
        public void Start(Rigidbody2D rigidbody, AudioSource audioSource)
        {
            m_Rigidbody = rigidbody;
            m_AudioSource = audioSource;

            movingHorizontally = startDirection.y == 0; // if player is starting to move hor or ver
            mainDirection = startDirection; // writing main direction to match the default direction
            RecalculateAdditionalDirectionOnRotation(false); // initially recalculating additional direction
            m_Rigidbody.AddForce((mainDirection + additionalDirection) * speed, ForceMode2D.Impulse); // adding initial force to player
        }

        /// <summary>
        /// Changes the additional direction (gravity) for player.
        /// Called when player touches the screen.
        /// </summary>
        public void TapOnScreen()
        {
            if (canJump) // if player is grounded, he jumps
            {
                canJump = false; // player shouldn't jump once again while in the air
                if (movingHorizontally)
                {
                    additionalDirection.y = additionalDirection.y == 1 ? -1 : 1; // changing additional direction to opposite one
                    additionalDirection.x = 0; // removing additional direction on wrong axis
                    offsetForRaycast = new Vector3(.2f * additionalDirection.x, .2f * -additionalDirection.y, 0); // calculating offset for raycasts that check wall
                }
                else
                {
                    additionalDirection.x = additionalDirection.x == 1 ? -1 : 1; // changing additional direction to opposite one
                    additionalDirection.y = 0; // removing additional direction on wrong axis
                    offsetForRaycast = new Vector3(.2f * -additionalDirection.x, .2f * additionalDirection.y, 0); // calculating offset for raycasts that check wall
                }
                m_AudioSource.Play(); // playing the audio of jump
                m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse); // adding force to new additional direction
            }
        }

        /// <summary>
        /// Calculates the hint vector offset from the player based on directions and rotates the vector towards the projection.
        /// </summary>
        /// <param name="HintVector"></param>
        /// <returns>The position of hint vector.</returns>
        public Vector3 hintVectorPositionAndRot(Transform HintVector)
        {
            Vector3 result; // vector to return
            if (movingHorizontally)
            {
                result = new Vector3(.4f * mainDirection.x, .4f * -additionalDirection.y, 0); // calculating offset
                HintVector.rotation = Quaternion.Euler(0, 0, -90 * mainDirection.x + 45 * -additionalDirection.y); // rotating vector
            }
            else
            {
                result = new Vector3(.4f * -additionalDirection.x, .4f * mainDirection.y, 0); // calculating offset
                if (mainDirection.y > 0)
                    HintVector.rotation = Quaternion.Euler(0, 0, 45 * additionalDirection.x); // rotating vector
                else
                    HintVector.rotation = Quaternion.Euler(0, 0, -180 + 45 * -additionalDirection.x); // rotating vector
            }
            return result;
        }

        /// <summary>
        /// <para>Changes the main direction when player ENTERS the rotation on level.</para>
        /// Changes the additional direction if player didn't jump before the rotation on level, <br/>
        /// otherwise the additional direction will be calculated later when player collides with level border.
        /// <para>Called from OnTriggerEnter of rotation element.</para>
        /// </summary>
        /// <param name="newDirection"></param>
        /// <param name="inverted"></param>
        public void SetMovementDirection(Vector3 newDirection, bool inverted)
        {
            movingHorizontally = newDirection.y == 0; // if player will move horizontally on next part of level
            mainDirection = newDirection; // new main direction
            if (canJump) // if player didn't jump before the rotation on level
            {
                canJump = false; // player shouldn't jump while he is in the rotation element
                RecalculateAdditionalDirectionOnRotation(inverted); // recalculating additional direction
            }
            else
            {
                waitingToChangeDirection = true; // indicates that the additional direction will be calculated later
                savedDirection = newDirection; // indicates that the main direction will be set later
                invertedRotation = inverted; // indicates whether the rotation element was inverted
            }
        }

        /// <summary>
        /// <para>Changes the main and additional direction when player collides with border.</para>
        /// <para>Called from OnTriggerEnter of rotation element.</para>
        /// </summary>
        public void SetMovementDirectionDelayed()
        {
            waitingToChangeDirection = false; // disabling so we don't calculate everything once again later

            RecalculateAdditionalDirectionOnRotation(!invertedRotation); // recalculating additional direction
            AddMainForceInDirection(savedDirection); // adding force to player in right direction
        }

        /// <summary>
        /// Pushing player in sum of main and additional direction to stick to border.
        /// </summary>
        /// <param name="newDirection"></param>
        public void AddMainForceInDirection(Vector3 newDirection)
        {
            if (!waitingToChangeDirection) // preventing from call when player exits the rotation element
            {
                mainDirection = newDirection; // setting main direction to the new one
                m_Rigidbody.velocity = Vector2.zero; // stopping player to add new force
                m_Rigidbody.AddForce((mainDirection + additionalDirection) * speed, ForceMode2D.Impulse); // adding force
                canJump = true; // giving ability to jump
            }
        }

        /// <summary>
        /// Whether player is moving horizontally.
        /// </summary>
        /// <returns>Bool if player is moving horizontally</returns>
        public bool GetMovingHorizontally()
        {
            return movingHorizontally;
        }

        /// <summary>
        /// Recalculates the additional direction when player is in rotation element.
        /// </summary>
        /// <param name="inverted">True if rotation is clockwise, otherwise false.</param>
        private void RecalculateAdditionalDirectionOnRotation(bool inverted)
        {
            if (movingHorizontally)
            {
                if (inverted)
                {
                    additionalDirection.y = additionalDirection.x == 1 ? 1 : -1; // calculating additional direction
                }
                else
                {
                    additionalDirection.y = additionalDirection.x == 1 ? -1 : 1; // calculating additional direction
                }
                additionalDirection.x = 0; // removing additional direction on wrong axis
                offsetForRaycast = new Vector3(.2f * mainDirection.x, .2f * -additionalDirection.y, 0); // calculating the offset for raycasts
            }
            else
            {
                if (inverted)
                {
                    additionalDirection.x = additionalDirection.y == 1 ? -1 : 1; // calculating additional direction
                }
                else
                {
                    additionalDirection.x = additionalDirection.y == 1 ? 1 : -1; // calculating additional direction
                }
                additionalDirection.y = 0; // removing additional direction on wrong axis
                offsetForRaycast = new Vector3(.2f * mainDirection.y, .2f * -additionalDirection.x, 0); // calculating the offset for raycasts
            }
        }

        /// <summary>
        /// Checks if there is a wall in additional direction to push player in the void.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>True if there is a border, otherwise false.</returns>
        public bool CheckWallInAdditionalDirection(Vector3 position)
        {
            if (canJump)
            {
                RaycastHit2D hitBack = Physics2D.Linecast(position + offsetForRaycast * .9f, position + offsetForRaycast * .9f + additionalDirection); // raycast on front of the player
                RaycastHit2D hitFront = Physics2D.Linecast(position - offsetForRaycast, position - offsetForRaycast + additionalDirection); // raycast on back of the player

                Debug.DrawLine(position + offsetForRaycast * .9f, position + offsetForRaycast * .9f + additionalDirection); // line on front of the player
                Debug.DrawLine(position - offsetForRaycast, position - offsetForRaycast + additionalDirection); // line on back of the player

                if (hitBack.collider == null && hitFront.collider == null) // if we didn't hit anything
                {
                    m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse); // add force to throw away player
                    canJump = false; // disable jump
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reset speed to desired value.
        /// </summary>
        public void ResetSpeed()
        {
            if (canJump)
            {
                if (movingHorizontally)
                {
                    if (Mathf.Abs(m_Rigidbody.velocity.x) != speed || m_Rigidbody.velocity.y != 0) // if speed is different
                    {
                        m_Rigidbody.velocity = Vector2.zero; // clear velocity
                        m_Rigidbody.AddForce(mainDirection * speed, ForceMode2D.Impulse); // add right force
                    }
                }
                else
                {
                    if (Mathf.Abs(m_Rigidbody.velocity.y) != speed || m_Rigidbody.velocity.x != 0) // if speed is different
                    {
                        m_Rigidbody.velocity = Vector2.zero; // clear velocity
                        m_Rigidbody.AddForce(mainDirection * speed, ForceMode2D.Impulse); // add right force
                    }
                }
            }
        }
    }

    public bool rotating;
    public GameObject DeathParticles;
    public Transform HintVector;
    public Transform HintDanger;
    public Transform Projection;
    [HideInInspector] public bool renderingVector = false;
    [HideInInspector] public bool renderingDanger = false;
    [HideInInspector] public Vector3 hintVectorPos;
    [HideInInspector] public bool movingCamera;
    [HideInInspector] public float t = 0f;
    [HideInInspector] public Transform mainCamera;
    [HideInInspector] public Vector3 mainCameraOldPos;
    [HideInInspector] public float mainMovementCoordinate;
    [HideInInspector] public float mainMovementDirection;
    [HideInInspector] public bool movingHorizontally;
    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public Rigidbody2D m_Rigidbody;
    [HideInInspector] public bool finished;
    [HideInInspector] public float endLevelBonus;
    [HideInInspector] public bool gameEnded;
    [HideInInspector] public Vector2 lastCheckpointPos;
    [HideInInspector] public Vector2 lastCheckpointMainDir;
    [HideInInspector] public Vector2 lastCheckpointAddDir;
    [HideInInspector] public float lastCheckpointSpeed;
    [HideInInspector] public bool startOfLevel;

    public MainMovement mMovement;

    void Awake()
    {
        startOfLevel = true;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        finished = false;
        mainCamera = Camera.main.transform;
        endLevelBonus = 1f;

        movingHorizontally = mMovement.startDirection.y == 0;
        mainMovementDirection = movingHorizontally ? mMovement.startDirection.x : mMovement.startDirection.y;
        mainMovementCoordinate = movingHorizontally ? transform.position.y : transform.position.x;
    }

    public void StartFromTap()
    {
        if (startOfLevel)
        {
            mMovement.Start(m_Rigidbody, GetComponent<AudioSource>());
            defineDifficulty();
            HintVisible(false);

            SetCheckpoint(transform.position, mMovement.startDirection, mMovement.additionalDirection, mMovement.speed, false);
            startOfLevel = false;
        }
    }

    private void Update()
    {
        UpdateCamera();
        CheckIfPlayerFlewAway();
        UpdateHints();
        CheckWallInAdditionalDirection();
    }

    private void FixedUpdate()
    {
        //CheckWallInAdditionalDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy") // when player collides with enemy
        {
            Death();
        }
        else // when player collides with ground
        {
            HintVisible(true);
            if (mMovement.waitingToChangeDirection)
            {
                mMovement.SetMovementDirectionDelayed();
                hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector);
            }
            else
            {
                if(!rotating)
                    mMovement.canJump = true;
            }
            AddRotation();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!rotating)
            mMovement.ResetSpeed();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (finished && mMovement.canJump)
        {
            //print("HERE");
            //mMovement.AddForceInAdditionalDirection();
        }
        ClearRotation();
    }

    public void TapOnScreen()
    {
        mMovement.TapOnScreen();
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector);
        HintVisible(false);
    }

    private void UpdateCamera()
    {
        if (!movingCamera) // main camera normal positioning
        {
            if (movingHorizontally)
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
            t += Time.deltaTime * (mMovement.speed / 4);
            Vector3 target;
            if (movingHorizontally)
            {
                target = new Vector3(transform.position.x + 1.5f * mainMovementDirection, mainMovementCoordinate, -10);
                mainCamera.position = Vector3.Lerp(mainCameraOldPos, target, t);
            }
            else
            {
                target = new Vector3(mainMovementCoordinate, transform.position.y + 3f * mainMovementDirection, -10);
                mainCamera.position = Vector3.Lerp(mainCameraOldPos, target, t);
            }
            if (mainCamera.position == target)
            {
                movingCamera = false;
            }
        }
    }

    private void UpdateHints()
    {
        if (rotating)
        {
            HintVisible(false);
        }
        else
        {
            if (mMovement.canJump)
            {
                if (renderingVector)
                {
                    HintVector.position = transform.position + hintVectorPos;

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, hintVectorPos, 10);

                    if (hit)
                    {
                        HintVisible(true);
                        Projection.transform.position = hit.point - new Vector2(hintVectorPos.x, hintVectorPos.y).normalized * Mathf.Sqrt(2) * .2f;
                    }
                    else
                    {
                        HintVisible(false);
                    }
                }
                if (renderingDanger)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, m_Rigidbody.velocity, 8);
                    if (hit.collider == null)
                    {
                        HintDanger.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (hit.collider.tag == "Enemy")
                        {
                            HintDanger.gameObject.SetActive(true);
                            if(movingHorizontally)
                                HintDanger.position = transform.position + new Vector3(0, .5f, 0);
                            else
                                HintDanger.position = transform.position + new Vector3(0, .5f * mMovement.mainDirection.y, 0);
                        }
                        else
                        {
                            HintDanger.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void CheckWallInAdditionalDirection()
    {
        if (!mMovement.CheckWallInAdditionalDirection(transform.position))
            HintVisible(false);
    }

    private void CheckIfPlayerFlewAway()
    {
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 pos = transform.position;
        if(!gameEnded && (pos.x > topRight.x || pos.x < bottomLeft.x || pos.y > topRight.y || pos.y < bottomLeft.y))
        {
            gameEnded = true;
            if(!finished)
                Death();
            else
                Finish();
        }
    }

    private void Death()
    {
        GameObject fx = Instantiate(DeathParticles, transform.position, Quaternion.Euler(0, -90, 0));
        fx.GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume;
        Destroy(fx, 2.5f);
        gameObject.SetActive(false);
        levelManager.Death();
    }

    public void Finish()
    {
        HintVisible(false);
        levelManager.Finish(endLevelBonus);
    }

    private void defineDifficulty()
    {
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector);
        switch (levelManager.difficulty)
        {
            case 0:
                HintVector.gameObject.SetActive(true);
                HintDanger.gameObject.SetActive(true);
                renderingVector = true;
                renderingDanger = true;
                break;
            case 1:
                HintVector.gameObject.SetActive(true);
                renderingVector = true;
                break;
        }
    }

    public void SetMovementDirection(Vector3 newDirection, float newMainCoord, bool inverted)
    {
        t = 0f;
        movingCamera = true;
        rotating = true;
        mainCameraOldPos = mainCamera.position;
        mMovement.SetMovementDirection(newDirection, inverted);
        movingHorizontally = mMovement.GetMovingHorizontally();
        mainMovementCoordinate = newMainCoord;
        HintVisible(false);
        Projection.gameObject.SetActive(false);
        if (movingHorizontally)
        {
            mainMovementDirection = newDirection.x;
        }
        else
        {
            mainMovementDirection = newDirection.y;
        }
    }

    public void AddForceInDirection(Vector3 newDirection)
    {
        rotating = false;

        mMovement.AddMainForceInDirection(newDirection);
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector);

        HintVisible(true);
        Projection.gameObject.SetActive(true);
    }

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

    public void SetSpeed(float newSpeed)
    {
        mMovement.speed = newSpeed;
    }

    public float GetSpeed()
    {
        return mMovement.speed;
    }

    public void AddRotation()
    {
        float speed = mMovement.speed;
        if (movingHorizontally)
            m_Rigidbody.angularVelocity = speed * (mMovement.additionalDirection.y == 1 ? 100 : -100);
        else
            m_Rigidbody.angularVelocity = speed * (mMovement.additionalDirection.x == 1 ? -100 : 100);
    }

    public void ClearRotation()
    {
        m_Rigidbody.angularVelocity = 0;
    }

    public void SetCheckpoint(Vector2 position, Vector2 mainDir, Vector2 additionalDir, float speed, bool inverted)
    {
        lastCheckpointPos = position;
        lastCheckpointMainDir = mainDir;
        lastCheckpointAddDir = additionalDir;
        lastCheckpointSpeed = speed;
    }

    public void RespawnOnLastCheckpoint()
    {
        gameObject.SetActive(true);
        levelManager.PrepareMainUI();
        transform.position = lastCheckpointPos;
        mMovement.additionalDirection = lastCheckpointAddDir;
        mMovement.speed = lastCheckpointSpeed;
        mMovement.AddMainForceInDirection(lastCheckpointMainDir);
    }
}
