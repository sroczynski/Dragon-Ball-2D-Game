using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : Assets.Scripts.ClientSocket
{
    public float walkSpeed = 1; // player left right walk speed
    private bool _isGrounded = true; // is player on the ground?

    Animator animator;

    //some flags to check when certain animations are playing
    bool _isPlaying_fly = false;
    bool _isPlaying_punch = false;
    bool _isPlaying_defend = false;
    bool _isPlaying_jump = false;

    //animation states - the values in the animator conditions
    const int STATE_IDLE = 0;
    const int STATE_FLY = 1;
    const int STATE_PUNCH = 2;
    const int STATE_DEFEND = 3;
    const int STATE_JUMP = 4;

    string _currentDirection = "left";
    int _currentAnimationState = STATE_IDLE;

    // Use this for initialization
    void Start()
    {
        //define the animator attached to the player
        animator = this.GetComponent<Animator>();
    }

    // FixedUpdate is used insead of Update to better handle the physics based jump
    void FixedUpdate()
    {
        //Check for keyboard input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeState(STATE_PUNCH);
        }
        else if (Input.GetKey("up") && !_isPlaying_punch && !_isPlaying_defend)
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                //simple jump code using unity physics
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 250));
                changeState(STATE_JUMP);
            }
        }
        else if (Input.GetKey("down"))
        {
            changeState(STATE_DEFEND);
        }
        else if (Input.GetKey("right") && !_isPlaying_punch)
        {
            changeDirection("right");
            transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);

            if (_isGrounded)
                changeState(STATE_FLY);

        }
        else if (Input.GetKey("left") && !_isPlaying_punch)
        {
            changeDirection("left");
            transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);

            if (_isGrounded)
                changeState(STATE_FLY);

        }
        else
        {
            if (_isGrounded)
                changeState(STATE_IDLE);
        }

        //check if crouch animation is playing
        _isPlaying_defend = animator.GetCurrentAnimatorStateInfo(0).IsName("defend");

        //check if hadooken animation is playing
        _isPlaying_punch = animator.GetCurrentAnimatorStateInfo(0).IsName("punch");

        //check if strafe animation is playing
        _isPlaying_fly = animator.GetCurrentAnimatorStateInfo(0).IsName("fly");
    }

    //--------------------------------------
    // Change the players animation state
    //--------------------------------------
    void changeState(int state)
    {
        if (_currentAnimationState == state)
            return;

        animator.SetInteger("state", state);

        _currentAnimationState = state;
    }

    //--------------------------------------
    // Check if player has collided with the floor
    //--------------------------------------
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "floor")
        {
            _isGrounded = true;
            changeState(STATE_IDLE);
        }
    }

    //--------------------------------------
    // Flip player sprite for left/right walking
    //--------------------------------------
    void changeDirection(string direction)
    {
        if (_currentDirection != direction)
        {
            if (direction == "right")
            {
                transform.Rotate(0, -180, 0);
                _currentDirection = "right";
            }
            else if (direction == "left")
            {
                transform.Rotate(0, 180, 0);
                _currentDirection = "left";
            }
        }
    }
}