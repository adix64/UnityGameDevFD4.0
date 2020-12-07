using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public float startHitBox = 0.03f;
    public float endHitBox = 0.2f;
    public HumanBodyBones hand;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {//hitbox activ doar in frame-urile in care mana este intinsa (startHitBox -> endHitBox)
        if (stateInfo.normalizedTime > startHitBox && stateInfo.normalizedTime < endHitBox)
            animator.GetBoneTransform(hand).GetComponent<SphereCollider>().enabled = true;
        else
            animator.GetBoneTransform(hand).GetComponent<SphereCollider>().enabled = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {// for safety, when forced to exit state
        animator.GetBoneTransform(hand).GetComponent<SphereCollider>().enabled = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
