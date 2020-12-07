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
    public float rotSpeed  = 20f;
    AnimatorStateInfo stateInfo;
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
        ApplyRootRotation();
        HandleAttack();
    }

    private void HandleAttack()
    {//ataca doar la mai putin de 1.5m fata de personaj...
        //...daca un nr aleator pica intr-un interval cu probabilitate mica
        float dist = Vector3.Distance(transform.position, player.position);
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (dist < attackDistThreshold && rand < 0.01f)
            animator.SetTrigger("Attack");
    }
    private void ApplyRootRotation()
    {
        if (agent.velocity.magnitude < 0.001)
            return; //rotim doar daca se misca si daca nu e in aer

        Vector3 lookDirection = (player.position - transform.position).normalized;
        // noua rotatie, ce se uita in directia de miscare sau catre inamic:
        Quaternion newRotation = Quaternion.LookRotation(lookDirection);
        // suprasscriem rotatia actuala, smoothly cu spherical liniar interpolation(SLERP)
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotSpeed);
    }
    private void UpdateAnimatorMoveParams()
    {
        Vector3 characterSpaceDir = transform.InverseTransformDirection(agent.velocity).normalized;
        animator.SetFloat("Forward", characterSpaceDir.z, 0.15f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.15f, Time.deltaTime);
    }
}
