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
        private AudioSource trailSound; // player's movement audio source

        private Vector3 savedDirection; // main direction that will be set after rotation
        private Vector3 offsetForRaycastFront; // offset for raycasts on front of the player that check wall to throw player away
        private Vector3 offsetForRaycastBack; // offset for raycasts on back of the player that check wall to throw player away

        private bool invertedRotation; // is player is in inverted rotation element

        /// <summary>
        /// Initial setup of player.
        /// Called when user touches the screen to start the level.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="audioSource"></param>
        public void Start(Rigidbody2D rigidbody, AudioSource audioSource, AudioSource trail)
        {
            m_Rigidbody = rigidbody;
            m_AudioSource = audioSource;
            trailSound = trail;

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
                    offsetForRaycastFront = new Vector3(.2f * mainDirection.x, .2f * additionalDirection.y, 0f); // calculating offset for raycasts that check wall
                    offsetForRaycastBack = new Vector3(.2f * -mainDirection.x, 0f, 0f); // calculating offset for raycasts that check wall
                }
                else
                {
                    additionalDirection.x = additionalDirection.x == 1 ? -1 : 1; // changing additional direction to opposite one
                    additionalDirection.y = 0; // removing additional direction on wrong axis
                    offsetForRaycastFront = new Vector3(.2f * additionalDirection.x, .2f * mainDirection.y, 0f); // calculating offset for raycasts that check wall
                    offsetForRaycastBack = new Vector3(0f, .2f * -mainDirection.y, 0f); // calculating offset for raycasts that check wall
                }
                trailSound.Pause();
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
                result = new Vector3(.4f * mainDirection.x, .4f * -additionalDirection.y, 0);
                if (mainDirection.x < 0)
                    HintVector.rotation = Quaternion.Euler(0, 0, 90 + 45 * additionalDirection.y);
                else
                    HintVector.rotation = Quaternion.Euler(0, 0, -90 + 45 * -additionalDirection.y);
            }
            else
            {
                result = new Vector3(.4f * -additionalDirection.x, .4f * mainDirection.y, 0);
                if (mainDirection.y > 0)
                    HintVector.rotation = Quaternion.Euler(0, 0, 45 * additionalDirection.x);
                else
                    HintVector.rotation = Quaternion.Euler(0, 0, -180 + 45 * -additionalDirection.x);
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
        public void SetMovementDirectionDelayed(bool passedOnlyStart)
        {
            waitingToChangeDirection = false; // disabling so we don't calculate everything once again later

            RecalculateAdditionalDirectionOnRotation(passedOnlyStart ? invertedRotation : !invertedRotation); // recalculating additional direction
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
        public void RecalculateAdditionalDirectionOnRotation(bool inverted)
        {
            if (movingHorizontally)
            {
                if (inverted)
                {
                    additionalDirection.y = additionalDirection.x == 1 ? 1 : -1; // calculating additional direction
                    //additionalDirection.y = additionalDirection.x; // changing additional direction
                }
                else
                {
                    additionalDirection.y = additionalDirection.x == 1 ? -1 : 1; // calculating additional direction
                    //additionalDirection.y = -additionalDirection.x; // changing additional direction
                }
                additionalDirection.x = 0; // removing additional direction on wrong axis
                offsetForRaycastFront = new Vector3(.2f * mainDirection.x, .2f * additionalDirection.y, 0f); // calculating the offset for raycasts
                offsetForRaycastBack = new Vector3(.2f * -mainDirection.x, 0f, 0f); // calculating the offset for raycasts
            }
            else
            {
                if (inverted)
                {
                    additionalDirection.x = additionalDirection.y == 1 ? -1 : 1; // calculating additional direction
                    //additionalDirection.x = -additionalDirection.y; // changing additional direction
                }
                else
                {
                    additionalDirection.x = additionalDirection.y == 1 ? 1 : -1; // calculating additional direction
                    //additionalDirection.x = additionalDirection.y; // changing additional direction
                }
                additionalDirection.y = 0; // removing additional direction on wrong axis
                offsetForRaycastFront = new Vector3(.2f * additionalDirection.x, .2f * mainDirection.y, 0f); // calculating the offset for raycasts
                offsetForRaycastBack = new Vector3(0f, .2f * -mainDirection.y, 0f); // calculating the offset for raycasts
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
                RaycastHit2D hitFront = Physics2D.Linecast(position + offsetForRaycastFront * .8f, position + offsetForRaycastFront * .8f + additionalDirection * 0.1f); // raycast on back of the player
                RaycastHit2D hitBack = Physics2D.Linecast(position + offsetForRaycastBack, position + offsetForRaycastBack + additionalDirection * 0.3f); // raycast on front of the player

                Debug.DrawLine(position + offsetForRaycastFront * .8f, position + offsetForRaycastFront * .8f + additionalDirection * 0.1f); // line on front of the player
                Debug.DrawLine(position + offsetForRaycastBack, position + offsetForRaycastBack + additionalDirection * 0.3f); // line on back of the player

                if (hitBack.collider == null && hitFront.collider == null) // if we didn't hit anything
                {
                    m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse); // add force to throw away player
                    trailSound.Pause();
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

        public void ResetSpeedOnRotation()
        {
            if (Mathf.Abs(m_Rigidbody.velocity.x) < 1 && Mathf.Abs(m_Rigidbody.velocity.y) < 1) // if speed is very low
            {
                m_Rigidbody.velocity = Vector2.zero; // clear velocity
                m_Rigidbody.AddForce(mainDirection * speed, ForceMode2D.Impulse); // add right force
            }
        }

        /// <summary>
        /// Continues to play the audio when player collides with ground.
        /// </summary>
        public void ContinueAudio()
        {
            if (!trailSound.isPlaying)
                trailSound.Play();
        }
    }

    public bool rotating; // if player is on a rotation element
    public GameObject DeathParticles; // death effect

    public Transform HintVector; // vector that indicates direction of next jump
    public Transform HintDanger; // sign that indicates an obstacle nearby
    public Transform Projection; // result of next jump
    [HideInInspector] public bool renderingVector = false; // if rendering vector
    [HideInInspector] public bool renderingDanger = false; // if rendering sign
    [HideInInspector] public Vector3 hintVectorPos; // position of hint vector

    [HideInInspector] public Transform mainCamera; // camera
    [HideInInspector] public bool movingCamera; // if we need to move camera on a rotation element
    [HideInInspector] public float t = 0f; // time for lerp of camera on rotation element
    [HideInInspector] public Vector3 mainCameraOldPos; // start of lerp on rotation element
    [HideInInspector] public float mainMovementCoordinate; // static coordinate for moving camera
    [HideInInspector] public float mainMovementDirection; // direction of player movement

    [HideInInspector] public bool movingHorizontally; // if player is moving horizontally
    [HideInInspector] public LevelManager levelManager; // manager
    [HideInInspector] public Rigidbody2D m_Rigidbody; // player's rigidbody
    [HideInInspector] public bool finished; // if player crossed the finish line
    [HideInInspector] public float endLevelBonus; // bonus at the end of level
    [HideInInspector] public bool gameEnded; // if the game has ended
    [HideInInspector] public bool startOfLevel; // if user started the game

    [HideInInspector] public Vector2 lastCheckpointPos; // position of last checkpoint
    [HideInInspector] public Vector2 lastCheckpointMainDir; // main direction on last checkpoint
    [HideInInspector] public Vector2 lastCheckpointAddDir; // additional direction on last checkpoint
    [HideInInspector] public float lastCheckpointSpeed; // speed on last checkpoint
    [HideInInspector] public bool lastCheckpointWasStart; // speed on last checkpoint
    [HideInInspector] public bool passedOnlyStart; // to solve the problem when player jumps in rotation from wrong side and crosses only start of rotation

    public MainMovement mMovement;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        mainCamera = Camera.main.transform;

        startOfLevel = true; // indicates that user didn't tap on screen for the first time
        lastCheckpointWasStart = true;
        finished = false; // indicates that player didn't finish
        endLevelBonus = 1f; // default bonus

        movingHorizontally = mMovement.startDirection.y == 0; // whether player starts to move horizontally
        mainMovementDirection = movingHorizontally ? mMovement.startDirection.x : mMovement.startDirection.y; // main direction at the start
        mainMovementCoordinate = movingHorizontally ? transform.position.y : transform.position.x; // main coordinate at the start
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
            mMovement.Start(m_Rigidbody, GetComponent<AudioSource>(), GetComponentsInChildren<AudioSource>()[1]); // initial player setup

            defineDifficulty(); // set difficulty
            HintVisible(false); // hide hints at the start

            SetCheckpoint(transform.position, mMovement.startDirection, mMovement.additionalDirection, mMovement.speed); // first checkpoint
            startOfLevel = false; // level started
        }
    }

    private void Update()
    {
        UpdateCamera(); // updating camera position
        CheckIfPlayerFlewAway(); // check if player flew away from the screen
        UpdateHints(); // hints positioning
        CheckWallInAdditionalDirection(); // check if we need to throw player in void
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy") // when player collides with enemy
        {
            Death();
        }
        else // when player collides with ground
        {
            HintVisible(true);
            mMovement.ContinueAudio();
            if (mMovement.waitingToChangeDirection) // if player collides after rotation
            {
                mMovement.SetMovementDirectionDelayed(passedOnlyStart); // setting new direction of movement
                hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector); // calculate position of vector
            }
            else
            {
                if (!rotating)
                    mMovement.canJump = true; //  player can jump if not in the rotation element
            }
            AddRotation(); // spinning player
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!rotating)
            mMovement.ResetSpeed(); // recalculate speed if not in rotation element
        else
            mMovement.ResetSpeedOnRotation(); // recalculate speed if in rotation element
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (finished && mMovement.canJump)
        //{
        //    //print("HERE");
        //    //mMovement.AddForceInAdditionalDirection();
        //}
        ClearRotation(); // stop player rotation after jump
    }

    /// <summary>
    /// Called when user touches the screen to jump.
    /// </summary>
    public void TapOnScreen()
    {
        mMovement.TapOnScreen(); // player jumps
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector); // updating vector
        HintVisible(false); // disabling hints
    }

    /// <summary>
    /// Updates camera position based on player position.
    /// </summary>
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
            t += Time.deltaTime * (mMovement.speed / 4); // time for lerp
            Vector3 target; // desired position for camera
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
            if (t > 1f) // if camera is in right position
            {
                movingCamera = false;
            }
        }
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
            if (mMovement.canJump) // if player is grounded
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
                                HintDanger.position = transform.position + new Vector3(0, .5f * mMovement.mainDirection.y, 0); // positioning sign
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
        if (!mMovement.CheckWallInAdditionalDirection(transform.position))
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
                Death();
        }
    }

    /// <summary>
    /// Death of player.
    /// </summary>
    private void Death()
    {
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
    private void defineDifficulty()
    {
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector); // initial position of vector
        switch (levelManager.difficulty)
        {
            case 0: // if easy difficulty
                HintVector.gameObject.SetActive(true);
                HintDanger.gameObject.SetActive(true);
                renderingVector = true;
                renderingDanger = true;
                break;
            case 1: // if medium difficulty
                HintVector.gameObject.SetActive(true);
                renderingVector = true;
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

        t = 0f; // reset timer for camera lerp
        movingCamera = true; // start rotation of the camera
        mainCameraOldPos = mainCamera.position; // start position of camera in rotation
        mainMovementCoordinate = newMainCoord; // update coordinate for camera

        rotating = true; // player is in rotation element
        mMovement.SetMovementDirection(newDirection, inverted); // change player direction
        movingHorizontally = mMovement.GetMovingHorizontally(); // update if player moves horizontally after rotation

        HintVisible(false);

        if (movingHorizontally)
        {
            mainMovementDirection = newDirection.x; // update direction for camera
        }
        else
        {
            mainMovementDirection = newDirection.y; // update direction for camera
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

        mMovement.AddMainForceInDirection(newDirection); // push player
        hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector); // update vector position

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
        mMovement.speed = newSpeed;
    }

    /// <summary>
    /// Tells player's speed
    /// </summary>
    /// <returns>Player's speed.</returns>
    public float GetSpeed()
    {
        return mMovement.speed;
    }

    /// <summary>
    /// Adds the spin effect to the player based on speed.
    /// </summary>
    public void AddRotation()
    {
        float speed = mMovement.speed;
        if (movingHorizontally)
            m_Rigidbody.angularVelocity = speed * (mMovement.additionalDirection.y == 1 ? 100 : -100);
        else
            m_Rigidbody.angularVelocity = speed * (mMovement.additionalDirection.x == 1 ? -100 : 100);
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
    public void SetCheckpoint(Vector2 position, Vector2 mainDir, Vector2 additionalDir, float speed)
    {
        if (!startOfLevel)
            lastCheckpointWasStart = false;
        lastCheckpointPos = position;
        lastCheckpointMainDir = mainDir;
        lastCheckpointAddDir = additionalDir;
        lastCheckpointSpeed = speed;
    }

    /// <summary>
    /// Respawns player on last checkpoint.
    /// </summary>
    public void RespawnOnLastCheckpoint()
    {
        if (!lastCheckpointWasStart)
        {
            gameObject.SetActive(true);
            GetComponentInChildren<TrailRenderer>().emitting = false;
            levelManager.PrepareMainUI();

            transform.position = lastCheckpointPos;
            mMovement.mainDirection = lastCheckpointMainDir;
            mMovement.additionalDirection = lastCheckpointAddDir;
            mMovement.speed = lastCheckpointSpeed;
            mMovement.movingHorizontally = mMovement.mainDirection.y == 0;
            mMovement.RecalculateAdditionalDirectionOnRotation(false);
            hintVectorPos = mMovement.hintVectorPositionAndRot(HintVector); // calculate position of vector

            movingHorizontally = mMovement.movingHorizontally; // whether player starts to move horizontally
            mainMovementDirection = movingHorizontally ? mMovement.mainDirection.x : mMovement.mainDirection.y; // main direction at the start
            mainMovementCoordinate = movingHorizontally ? transform.position.y : transform.position.x; // main coordinate at the start

            mMovement.AddMainForceInDirection(lastCheckpointMainDir);

            foreach(GhostEnemy enemy in FindObjectsOfType<GhostEnemy>())
            {
                enemy.RestartEnemy();
            }
        }
        else
        {
            levelManager.RestartLevel(false);
        }
    }
}