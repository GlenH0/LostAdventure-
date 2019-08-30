using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{

    public float moveSpeed;
    //public Rigidbody theRB;
    public float jumpForce;
    public CharacterController controller;
    public Text puzzleText;
    public Text puzzleValid;
    private Vector3 moveDirection;
    public float gravityScale;

    int[] platformArray = new int[] { 1, 2, 3, 4 };
    int[] platformArrayShuffled = new int[] { 1, 2, 3, 4 };

     int tracker = 1;

    private block block; //Stores the Block Script

    public Rigidbody rb;

    public Animator anim;
    public Transform pivot;
    public float rotateSpeed; //for player rotation
    public GameObject playerModel;

    AudioSource audioSrc;
    bool isMoving = false;
    

    // Start is called before the first frame update
    void Start()
    {
        //theRB = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        //Find Object of Type = Finds an object in the scene with the block script
        //Use Find Object of Type sparingly as it is very expensive, and probably only use it in Start and Awake Functions
        //This function will store a reference to the script of the block object
        block = FindObjectOfType<block>();

        for (int i = 0; i < 4; i++)
        {
            int si = Random.Range(0, 4 - i);
            int temp = platformArrayShuffled[i];
            platformArrayShuffled[i] = platformArrayShuffled[si + i];
            platformArrayShuffled[si + i] = temp;
        }
        for (int i = 0; i < 4; i++)
        {
            print(platformArrayShuffled[i]);
        }

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //theRB.velocity = new Vector3(Input.GetAxis("Horizontal")*moveSpeed, theRB.velocity.y, Input.GetAxis("Vertical")*moveSpeed);

        /*  if (Input.GetButtonDown("Jump"))
          {
              //theRB.velocity = new Vector3(theRB.velocity.x, jumpForce, theRB.velocity.z);
          }
          */
        //moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, Input.GetAxis("Vertical") * moveSpeed);
        float yStore = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;
        
        //footstep audio
        if (moveDirection.x != 0 && controller.isGrounded)
        {
            isMoving = true;
        }
        else
            isMoving = false;
        if (isMoving)
        {
            if (!audioSrc.isPlaying)
                audioSrc.Play();
        }
        else
        {
            audioSrc.Stop();
        }
            

        /*----------------------------------------------------------------------------------*/

        if (controller.isGrounded)
        {
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //Move player in different directions base on camera look direction
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f); //change y only.
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f,moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        //if char fall, char dies.
        if (rb.position.y < -1f)
        {
            FindObjectOfType<GameManager>().EndGame();

        }

        //For animation
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));

        
    }

    private void RemoveBlock()
    {
        //Disables the collider of the Block by getting component of the Box Collider
        block.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    //for puzzle
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cube")
        {
            print(other.GetComponent<puzzle>().CubeNum + " + " + platformArrayShuffled[other.GetComponent<puzzle>().CubeNum - 1]);
            if (tracker <= 4)
            {
                puzzleText.text = (platformArrayShuffled[other.GetComponent<puzzle>().CubeNum - 1]).ToString();





                if (tracker == platformArrayShuffled[other.GetComponent<puzzle>().CubeNum - 1])
                {
                    tracker++;
                    puzzleValid.text = ("Almost there...");

                    if (tracker == 5)
                    {

                        print("Good Game!");
                        puzzleValid.text = ("YES! I DID IT! Let's get out of here!");
                        RemoveBlock();
                        other.GetComponent<Collider>().enabled = false;
                    }

                }

                else
                {
                    tracker = 1;
                    print("tracker reset!");
                    puzzleValid.text = ("Darn it! The puzzle seems to have reset!");



                }
            }

            
        }
    }
}
