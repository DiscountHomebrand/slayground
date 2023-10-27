using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrisonerScript : MonoBehaviour {
    private Vector3 mTarget;

    // Start is called before the first frame update
    void Start() {
        mTarget = transform.position;
    }

    public void MoveTo(Vector3 position) {
        // Set Target Position
        mTarget = position + Vector3.up * 0.1f;
    }

    // Update is called once per frame
    void Update() {
        // Move Straight towards Target
        transform.position = Vector3.MoveTowards(transform.position, mTarget, 3.5f * Time.deltaTime);
    }
}
