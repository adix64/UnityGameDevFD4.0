using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public string opponentLayer;
    public string side;
    public int damage = 5;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(opponentLayer))
        {//hitbox a lovit hurboxul adversarului
            var opponentAnimator = other.GetComponentInParent<Animator>();
            if (opponentAnimator.GetBool("Dead"))
                return;//daca e mort, nu-l mai pune sa sufere hit
            opponentAnimator.Play("TakeHit" + side);
            opponentAnimator.SetInteger("takenDamage", damage);
        }
    }
}
