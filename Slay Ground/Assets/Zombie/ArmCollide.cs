using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCollide : MonoBehaviour
{
    bool attacking;
    // Start is called before the first frame update
    void Start()
    {
        attacking=false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {   
        Debug.Log("Col tag: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player col");
            if (attacking){
                CharController_Motor playerScript = collision.gameObject.GetComponent<CharController_Motor>();
                playerScript.Damage(50);
            }
           
        }
    }

    public void SetAttacking (bool att){
        attacking = att;
    }
}
