using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 10f;

    [SerializeField]
    private float jumpForce = 11f;

    [SerializeField]
    private AudioClip death_rattle;

    [SerializeField]
    private AudioClip jump_sound;

    private AudioSource _audioSource;
    private float movementX;
    private string WALK_ANIMATION = "Walk";
    private string GROUND_TAG = "Ground";
    private string ENEMY_TAG = "Enemy";
    
    private Rigidbody2D myBody;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isGrounded;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.Log("Audio source not found.");
        }
        else
        {
            _audioSource.clip = jump_sound;
        }
    }

    void Update()
    {
        PlayerMoveKB();
        AnimatePlayer();
    }

    private void FixedUpdate()
    {
        PlayerJump();
    }

    void PlayerMoveKB()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void AnimatePlayer()
    {
        // player moving to the right
        if (movementX > 0)
        {
            sr.flipX = false;
            anim.SetBool(WALK_ANIMATION, true);
        }

        // player moving to the left
        else if (movementX < 0)
        {
            sr.flipX = true;
            anim.SetBool(WALK_ANIMATION, true);
        }

        // play idle
        else
        {
            anim.SetBool(WALK_ANIMATION, false);
        }
    }

    void PlayerJump()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            _audioSource.PlayOneShot(jump_sound);
            isGrounded = false;
            myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(GROUND_TAG))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag(ENEMY_TAG))
        {
            _audioSource.clip = death_rattle;
            _audioSource.Play();
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, _audioSource.clip.length);
        }
    }
}
