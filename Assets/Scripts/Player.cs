using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] [Range(0f, 5f)] float hitVolume = 5f;

    Vector2 rawInput;
    Vector2 minBounds;
    Vector2 maxBounds;

    Shooter shooter;

    float shotDelay = 0.05f;

    GameObject playerRig;
    Animator myAnimator;
    Animation isSendingNotes;
    Rigidbody2D playerRigidBody;
    PlayerSpecial playerSpecial;
    SpriteRenderer[] ensembleBalloon;

    void Awake()
    {
        shooter = GetComponent<Shooter>();
    }

    void Start()
    {
        InitBounds();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerRig = playerRigidBody.transform.GetChild(0).gameObject;
        myAnimator = playerRig.GetComponent<Animator>();
        isSendingNotes = playerRig.GetComponent<Animation>();
        playerSpecial = FindObjectOfType<PlayerSpecial>();
        ensembleBalloon = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        Move();
    }

    void InitBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0,0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1,1));
    }

    void Move()
    {
        Vector2 delta = rawInput * moveSpeed * Time.deltaTime;
        Vector2 newPos = new Vector2();
        newPos.x = Mathf.Clamp(transform.position.x + delta.x, minBounds.x + paddingLeft, maxBounds.x - paddingRight);
        newPos.y = Mathf.Clamp(transform.position.y + delta.y, minBounds.y + paddingBottom, maxBounds.y - paddingTop);
        transform.position = newPos;
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        IEnumerator SendNotes()
        {
            myAnimator.SetBool("isSendingNotes", true);
            shooter.isFiring = value.isPressed;

            yield return new WaitForSecondsRealtime(shotDelay);
            
            myAnimator.SetBool("isSendingNotes", false);
            myAnimator.SetBool("isIdling", true);
        }

        if(shooter != null) { StartCoroutine(SendNotes()); }
    }

    void OnReflect(InputValue value)
    {
        IEnumerator ReflectNotes()
        {
            myAnimator.SetBool("isTempo", true);
            StartCoroutine(ShowEnsembleBallon());
            shooter.isReflecting = value.isPressed;

            yield return new WaitForSecondsRealtime(0.30f);

            myAnimator.SetBool("isTempo", false);
            myAnimator.SetBool("isIdling", true);

            shooter.isReflecting = false;
        }

        if (shooter != null && playerSpecial.GetSpecial()) {
            StartCoroutine(ReflectNotes());
            playerSpecial.ResetSpecial();
        }
    }

    IEnumerator ShowEnsembleBallon()
    {
        ensembleBalloon[2].enabled = true;

        yield return new WaitForSecondsRealtime(1);

        ensembleBalloon[2].enabled = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 cameraPos = Camera.main.transform.position;

        string[] arrivingNoteProjectile = other.name.Split("_");

        if (arrivingNoteProjectile != null & arrivingNoteProjectile.Length > 1)
        {
            string noteNameToPlay = arrivingNoteProjectile[1].Split('(')[0].TrimEnd().ToLower();

            for (int i = 0; i < this.audioClips.Count; i++)
            {
                if (this.audioClips[i].name != null && this.audioClips[i].name == noteNameToPlay)
                {
                    AudioSource.PlayClipAtPoint(this.audioClips[i], cameraPos, hitVolume);
                }
            }
        }
    }
}
