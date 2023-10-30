using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ZombieNav : MonoBehaviour
{
    NavMeshAgent navMesh;
    public GameObject player;
    Vector3 mVelocity ;
    bool attacking = false;

    public float health = 100f;

    int currency = 5;

    CharacterController mController;
    // Start is called before the first frame update
    void Start()
    {
        mController = GetComponent<CharacterController>();
        // mVelocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player");
      
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updateRotation = true;
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
                if (distance >=2.0){
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
                    attacking = true;
                }
            }else{
                //if atacking animation finished nextline
                attacking=false;
            }
            
        }else{
            // play die animation
            // if die animation done nextline
            Destroy(gameObject,1);
        }
    }

    public void Headshot(){
        health -= 50f;
    }

    public void Damage(){
        health -=10f;
    }

    public bool IsDead() {
        return health <= 0;
    }

    public int GetCurrency() {
        return currency;
    }
}
