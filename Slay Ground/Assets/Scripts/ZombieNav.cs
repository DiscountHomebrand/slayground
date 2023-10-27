using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieNav : MonoBehaviour
{
    NavMeshAgent navMesh;
    public GameObject player;
    Vector3 mVelocity ;

    CharacterController mController;
    // Start is called before the first frame update
    void Start()
    {
        mController = GetComponent<CharacterController>();
       mVelocity = Vector3.zero;
       player = GameObject.FindGameObjectWithTag("Player");
       navMesh.updateRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.SetDestination(player.transform.position);
        
        

        if (navMesh.velocity.magnitude > 1){
            Quaternion targetRotation = Quaternion.LookRotation(navMesh.velocity);
            transform.rotation = targetRotation;
            
        }else{
            transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z));
        }
       
      
       
       /*
       if(mController.isGrounded == false) {
            mVelocity.y -= 9.8f * Time.deltaTime;
        } else {
            mVelocity.y = -2f;
        }
        */
        navMesh.Move((mVelocity)*Time.deltaTime);
    }
}
