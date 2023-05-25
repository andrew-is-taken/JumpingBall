using System;
using UnityEngine;

[Serializable]
public class MovementController
{
    public float speed = 8f; // current desired speed of player

    public Vector3 startDirection; // direction of player when level starts
    [HideInInspector] public Vector3 mainDirection; // main direction of player movement
    [HideInInspector] public Vector3 additionalDirection = new Vector3(1, 0, 0); // additional direction of player movement

    [HideInInspector] public bool movingHorizontally; // if player is currently moving horizontally

    [HideInInspector] public bool canJump; // if player can jump
    [HideInInspector] public bool waitingToChangeDirection; // if player needs to recalculate directions after rotation when he collides with border

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
    public void ChangeDirection()
    {
        if (canJump) // if player is grounded, he jumps
        {
            canJump = false; // player shouldn't jump once again while in the air
            RecalculateAdditionalDirectionOnJump();
            trailSound.Pause();
            m_AudioSource.Play(); // playing the audio of jump
            m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse); // adding force to new additional direction
        }
    }

    public void RecalculateAdditionalDirectionOnJump()
    {
        if (movingHorizontally)
        {
            additionalDirection.y = additionalDirection.y == 1 ? -1 : 1; // changing additional direction to opposite one
            additionalDirection.x = 0; // removing additional direction on wrong axis
        }
        else
        {
            additionalDirection.x = additionalDirection.x == 1 ? -1 : 1; // changing additional direction to opposite one
            additionalDirection.y = 0; // removing additional direction on wrong axis
        }
        CalculateOffsetForRaycasts();
    }

    private void CalculateOffsetForRaycasts()
    {
        if (movingHorizontally)
        {
            offsetForRaycastFront = new Vector3(.2f * mainDirection.x, .2f * additionalDirection.y, 0f); // calculating the offset for raycasts
            offsetForRaycastBack = new Vector3(.2f * -mainDirection.x, 0f, 0f); // calculating the offset for raycasts
        }
        else
        {
            offsetForRaycastFront = new Vector3(.2f * additionalDirection.x, .2f * mainDirection.y, 0f); // calculating offset for raycasts that check wall
            offsetForRaycastBack = new Vector3(0f, .2f * -mainDirection.y, 0f); // calculating offset for raycasts that check wall
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
            //offsetForRaycastFront = new Vector3(.2f * mainDirection.x, .2f * additionalDirection.y, 0f); // calculating the offset for raycasts
            //offsetForRaycastBack = new Vector3(.2f * -mainDirection.x, 0f, 0f); // calculating the offset for raycasts
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
            //offsetForRaycastFront = new Vector3(.2f * additionalDirection.x, .2f * mainDirection.y, 0f); // calculating the offset for raycasts
            //offsetForRaycastBack = new Vector3(0f, .2f * -mainDirection.y, 0f); // calculating the offset for raycasts
        }
        CalculateOffsetForRaycasts();
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

            //Debug.DrawLine(position + offsetForRaycastFront * .8f, position + offsetForRaycastFront * .8f + additionalDirection * 0.1f); // line on front of the player
            //Debug.DrawLine(position + offsetForRaycastBack, position + offsetForRaycastBack + additionalDirection * 0.3f); // line on back of the player

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

    public void SetMovingHorizontally()
    {
        movingHorizontally = mainDirection.y == 0;
    }
}
