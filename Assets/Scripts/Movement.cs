using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    [Serializable]
    public class MainMovement
    {
        public float speed = 8f;

        public Vector3 startDirection;
        public Vector3 additionalDirection = new Vector3(1, 0, 0);

        private bool movingHorizontally;

        public bool canJump;
        public bool waitingToChangeDirection;

        private Rigidbody2D m_Rigidbody;
        private AudioSource m_AudioSource;

        private Vector3 savedDirection;
        private Vector3 offsetForRaycast;

        public void Start(Rigidbody2D rigidbody, AudioSource audioSource)
        {
            m_Rigidbody = rigidbody;
            m_AudioSource = audioSource;
            movingHorizontally = startDirection.y == 0; // if player is starting to move hor or ver
            if (movingHorizontally)
            {
                additionalDirection.y = additionalDirection.x == 1 ? -1 : 1;
                additionalDirection.x = 0;
                offsetForRaycast = new Vector3(-.2f, .2f * additionalDirection.y, 0);
            }
            else
            {
                additionalDirection.x = additionalDirection.y == 1 ? 1 : -1;
                additionalDirection.y = 0;
                offsetForRaycast = new Vector3(.2f * additionalDirection.x, -.2f, 0);
            }
            m_Rigidbody.AddForce((startDirection + additionalDirection) * speed, ForceMode2D.Impulse);
        }

        public void TapOnScreen()
        {
            if (canJump) // if player is grounded, he jumps
            {
                canJump = false;
                if (movingHorizontally)
                {
                    additionalDirection.y = additionalDirection.y == 1 ? -1 : 1;
                    additionalDirection.x = 0;
                    offsetForRaycast = new Vector3(-.2f, .2f * additionalDirection.y, 0);
                }
                else
                {
                    additionalDirection.x = additionalDirection.x == 1 ? -1 : 1;
                    additionalDirection.y = 0;
                    offsetForRaycast = new Vector3(.2f * additionalDirection.x, -.2f, 0);
                }
                m_AudioSource.Play();
                m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse);
            }
        }

        public Vector3 hintVectorPositionAndRot(Transform HintVector)
        {
            Vector3 result;
            if (movingHorizontally)
            {
                result = new Vector3(.7f, .7f * -additionalDirection.y, 0);
                HintVector.rotation = Quaternion.Euler(0, 0, -90 + 45 * -additionalDirection.y);
            }
            else
            {
                result = new Vector3(.7f * -additionalDirection.x, .7f, 0);
                HintVector.rotation = Quaternion.Euler(0, 0, 45 * additionalDirection.x);
            }
            return result;
        }

        public void SetMovementDirection(Vector3 newDirection, float newMainCoord)
        {
            movingHorizontally = newDirection.y == 0;
            if (canJump)
            {
                canJump = false;
                if (movingHorizontally)
                {
                    additionalDirection.y = additionalDirection.x == 1 ? -1 : 1;
                    additionalDirection.x = 0;
                    offsetForRaycast = new Vector3(-.2f, .2f * additionalDirection.y, 0);
                }
                else
                {
                    additionalDirection.x = additionalDirection.y == 1 ? 1 : -1;
                    additionalDirection.y = 0;
                    offsetForRaycast = new Vector3(.2f * additionalDirection.x, -.2f, 0);
                }
            }
            else
            {
                waitingToChangeDirection = true;
                savedDirection = newDirection;
            }
        }

        public void SetMovementDirectionDelayed()
        {
            waitingToChangeDirection = false;
            if (movingHorizontally)
            {
                additionalDirection.y = additionalDirection.x == 1 ? -1 : 1;
                additionalDirection.x = 0;
                offsetForRaycast = new Vector3(-.2f, .2f * additionalDirection.y, 0);
            }
            else
            {
                additionalDirection.x = additionalDirection.y == 1 ? 1 : -1;
                additionalDirection.y = 0;
                offsetForRaycast = new Vector3(.2f * additionalDirection.x, -.2f, 0);
            }
            m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.AddForce((savedDirection + additionalDirection) * speed, ForceMode2D.Impulse);
            canJump = true;
        }

        public void AddMainForceInDirection(Vector3 newDirection)
        {
            if (!waitingToChangeDirection)
            {
                m_Rigidbody.velocity = Vector2.zero;
                m_Rigidbody.AddForce((newDirection + additionalDirection) * speed, ForceMode2D.Impulse);
                canJump = true;
            }
            else
            {
                waitingToChangeDirection = false;
                if (movingHorizontally)
                {
                    additionalDirection.y = additionalDirection.x == 1 ? 1 : -1;
                    additionalDirection.x = 0;
                    offsetForRaycast = new Vector3(-.2f, .2f * additionalDirection.y, 0);
                }
                else
                {
                    additionalDirection.x = additionalDirection.y == 1 ? -1 : 1;
                    additionalDirection.y = 0;
                    offsetForRaycast = new Vector3(.2f * additionalDirection.x, -.2f, 0);
                }
                m_Rigidbody.velocity = Vector2.zero;
                m_Rigidbody.AddForce((savedDirection + additionalDirection) * speed, ForceMode2D.Impulse);
                canJump = true;
            }
        }

        public void AddForceInAdditionalDirection()
        {
            //m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse);
            canJump = false;
        }

        public bool GetMovingHorizontally()
        {
            return movingHorizontally;
        }

        public bool CheckWallInAdditionalDirection(Vector3 position)
        {
            if (canJump)
            {
                RaycastHit2D hitBack = Physics2D.Linecast(position + offsetForRaycast, position + offsetForRaycast + additionalDirection);
                RaycastHit2D hitFront = Physics2D.Linecast(position - offsetForRaycast, position - offsetForRaycast + additionalDirection);
                Debug.DrawLine(position + offsetForRaycast, position + offsetForRaycast + additionalDirection);
                Debug.DrawLine(position - offsetForRaycast, position - offsetForRaycast + additionalDirection);
                if (hitBack.collider == null && hitFront.collider == null)
                {
                    m_Rigidbody.AddForce(additionalDirection * speed, ForceMode2D.Impulse);
                    canJump = false;
                    return false;
                }
            }
            return true;
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

    public MainMovement mMovement;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        mMovement.Start(m_Rigidbody, GetComponent<AudioSource>());
        levelManager = FindObjectOfType<LevelManager>();
        defineDifficulty();
        HintVisible(false);
        finished = false;
        mainCamera = Camera.main.transform;
        movingHorizontally = mMovement.GetMovingHorizontally();
        mainMovementDirection = movingHorizontally ? mMovement.startDirection.x : mMovement.startDirection.y;
        mainMovementCoordinate = movingHorizontally ? transform.position.y : transform.position.x;
        endLevelBonus = 1f;
    }

    private void Update()
    {
        UpdateCamera();
        CheckIfPlayerFlewAway();
        UpdateHints();
    }

    private void FixedUpdate()
    {
        CheckWallInAdditionalDirection();
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (finished && mMovement.canJump)
        {
            mMovement.AddForceInAdditionalDirection();
        }
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
        if (mMovement.canJump)
        {
            if (renderingVector)
            {
                HintVector.position = transform.position + hintVectorPos;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, hintVectorPos, 10);

                if (hit)
                {
                    //Projection.gameObject.SetActive(true);
                    //HintVector.gameObject.SetActive(true);
                    HintVisible(true);
                    Projection.transform.position = hit.point - new Vector2(hintVectorPos.x, hintVectorPos.y) * Mathf.Sqrt(2) * .2f;
                }
                else
                {
                    //Projection.gameObject.SetActive(false);
                    //HintVector.gameObject.SetActive(false);
                    HintVisible(false);
                }
            }
            if (renderingDanger)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, m_Rigidbody.velocity, 8);
                //Debug.DrawLine(transform.position, hit.point, Color.green);
                if (hit.collider == null)
                {
                    HintDanger.gameObject.SetActive(false);
                }
                else
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        HintDanger.gameObject.SetActive(true);
                        HintDanger.position = transform.position + new Vector3(0, .5f, 0);
                    }
                    else
                    {
                        HintDanger.gameObject.SetActive(false);
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
        Destroy(fx, 5);
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

    public void SetMovementDirection(Vector3 newDirection, float newMainCoord)
    {
        t = 0f;
        movingCamera = true;
        rotating = true;
        mainCameraOldPos = mainCamera.position;
        mMovement.SetMovementDirection(newDirection, newMainCoord);
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
}
