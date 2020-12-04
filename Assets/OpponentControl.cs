using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OpponentControl : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    Animator animator;
    public float attackDistThreshold = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;
        UpdateAnimatorMoveParams();
        HandleAttack();
    }

    private void HandleAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < attackDistThreshold)
            animator.SetTrigger("Attack");
    }

    private void UpdateAnimatorMoveParams()
    {
        Vector3 characterSpaceDir = transform.InverseTransformDirection(agent.velocity).normalized;
        animator.SetFloat("Forward", characterSpaceDir.z, 0.15f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.15f, Time.deltaTime);
    }
}
