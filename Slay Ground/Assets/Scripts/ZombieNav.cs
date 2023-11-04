using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ZombieNav : MonoBehaviour
{
    NavMeshAgent navMesh;
    public GameObject player;
    Vector3 mVelocity ;
    bool attacking = false;
    bool lastHeadshot = false;

    public float health = 100f;


    public Animator animator;

    ArmCollide[] armChildren ;

    int currency = 5;


    CharacterController mController;
    public bool dead= false;
    // Start is called before the first frame update
    void Start()
    {
        mController = GetComponent<CharacterController>();
        // mVelocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player");
      
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updateRotation = true;
        animator = GetComponent<Animator>();
        armChildren = GetComponentsInChildren<ArmCollide>();

    }

    // Update is called once per frame
    void Update()
    {   
       /*
       if(mController.isGrounded == false) {
            mVelocity.y -= 9.8f * Time.deltaTime;
        } else {
            mVelocity.y = -2f;
        }
        */
       // navMesh.Move((mVelocity)*Time.deltaTime);
    }
    private void FixedUpdate() {

        float distance = Vector3.Distance(player.transform.position,transform.position);
        if (!IsDead()){

            if (attacking==false){
                if (distance >=1.4f){
                    Quaternion targetRotation = Quaternion.LookRotation(navMesh.velocity);
                    transform.rotation = targetRotation;
                    navMesh.SetDestination(player.transform.position);  
                }else{
                    Vector3 whereToLook = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    Quaternion targetRotation = Quaternion.LookRotation(whereToLook - transform.position);

                    // Calculate the rotation step based on the max rotation speed.
                    float step = 240f * Time.deltaTime;

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
                    navMesh.velocity =  Vector3.zero;

                    //insert attacking animation
                    animator.SetTrigger("Attack");
                    attacking = true;
                    //set arm attacker
                    animator.applyRootMotion = true;
                    foreach (ArmCollide child in armChildren){
                        child.SetAttacking(attacking);    
                    }
                }
            }else { //currently swinging

            }
            
        }else{  
            attacking = false;
            animator.applyRootMotion = false;
            foreach (ArmCollide child in armChildren){
                child.SetAttacking(attacking);    
            }

            // play die animation
            if (lastHeadshot){ // last shot was headshot
                Transform zHead = transform.Find("Z_Head");
                if (zHead != null){
                    Destroy(zHead.gameObject);
                }
                animator.SetTrigger("Headshot");
            }else{//last shot was bodyshot
                animator.SetTrigger("Bodyshot");
            }
            
        }
    }

    public void Headshot(){
        health -= 50f;
        lastHeadshot = true;
    }

    public void Damage(){
        health -=10f;
        lastHeadshot = false;
    }

    public void AttackingFinished(){
        attacking = false;
        animator.applyRootMotion = false;

       foreach (ArmCollide child in armChildren){
            child.SetAttacking(attacking);    
        }
       
    }

    public void FinishedDying(){
        Destroy(gameObject,1);
    }

    public bool IsDead() {
        return health <= 0;
    }

    public int GetCurrency() {
        return currency;
    }
}
