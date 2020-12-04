using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public string opponentLayer;
    public string side;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(opponentLayer))
        {
            other.GetComponentInParent<Animator>().Play("TakeHit" + side);
        }
    }
}
