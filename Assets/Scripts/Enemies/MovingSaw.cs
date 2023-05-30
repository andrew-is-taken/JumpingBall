using UnityEngine;

public class MovingSaw : MonoBehaviour, ILevelObject
{
    [SerializeField] private float speed = 1f; // speed of saw
    [Range(-.6f, .6f)][SerializeField] private float startPositionX; // position of saw on start from -.6 to .6
    [SerializeField] private bool movingUp; // if the saw is moving up
    [SerializeField] private bool hasPauseOnEnd; // if the saw needs to stop a fter movement

    private Transform child; // saw
    private float t; // time for move lerp

    private bool waiting;
    private float wait; // time waiting
    private bool currMovingUp; // if the saw is moving up

    private void Awake()
    {
        child = GetComponentInChildren<Animator>().transform;
    }

    private void OnEnable()
    {
        currMovingUp = movingUp;
        child.localPosition = new Vector3 (startPositionX, 0, 0);
        t = (startPositionX + 0.6f) / 1.2f;
        if (!movingUp)
            t = 1 - t;
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Lerps the position of the saw between -0.6f and 0.6f on x axis.
    /// </summary>
    private void Move()
    {
        if (hasPauseOnEnd)
        {
            if (!waiting)
            {
                if (movingUp)
                    child.localPosition = new Vector3(Mathf.Lerp(-0.6f, 0.6f, t), 0, 0);
                else
                    child.localPosition = new Vector3(Mathf.Lerp(0.6f, -0.6f, t), 0, 0);

                t += Time.deltaTime * speed;
                if (t >= 1f)
                {
                    waiting = true;
                    wait = 0f;
                }
            }
            else
            {
                wait += Time.deltaTime * speed;
                if (wait >= 0.5f)
                {
                    waiting = false;
                    currMovingUp = !currMovingUp;
                    t = 0f;
                }
            }
        }
        else
        {
            if (currMovingUp)
                child.localPosition = new Vector3(Mathf.Lerp(-0.6f, 0.6f, t), 0, 0);
            else
                child.localPosition = new Vector3(Mathf.Lerp(0.6f, -0.6f, t), 0, 0);

            t += Time.deltaTime * speed;
            if (t >= 1f)
            {
                currMovingUp = !currMovingUp;
                t = 0f;
            }
        }
    }

    public void restartObject()
    {
        enabled = true;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void turnOffObject()
    {
        enabled = false;
    }
}
