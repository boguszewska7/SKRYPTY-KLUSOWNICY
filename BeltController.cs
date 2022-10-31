using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltController : MonoBehaviour
{
    private Rigidbody Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        Physics.SyncTransforms();
        if (collision.gameObject.CompareTag("tarcza"))
        { Physics.SyncTransforms();
            Rb.isKinematic = true;

        }
    }
}
