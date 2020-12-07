using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkDown : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {// declansat pentru 2 obiectie in coliziune care au collidere non-trigger si rigidbodies atasate
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.localScale = transform.localScale * 0.75f;
        }
    }
}
