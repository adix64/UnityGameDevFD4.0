using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float playerSpeed = 3f;
    public float rotSpeed = 10f;
    public float jumpPower = 5f;
    public float groundedThreshold = .15f;
    Transform cameraTransform;
    Rigidbody rigidbody;
    CapsuleCollider capsule;
    Vector3 moveDir;
    Animator animator;
    AnimatorStateInfo stateInfo;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        GetMoveDirection();
        UpdateAnimatorParameters();
        ApplyRootRotation();
        HandleJump();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void UpdateAnimatorParameters()
    {
        Vector3 characterSpaceDir = transform.InverseTransformDirection(moveDir);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.15f, Time.fixedDeltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.15f, Time.fixedDeltaTime);
    }


    private void HandleJump()
    {
        bool grounded = false; //presupun ca nu e pe pamant
        Ray ray = new Ray();
        ray.direction = Vector3.down;
        // 9 raze de deasupra bazei capsulei, aruncate in jos, detecteaza pamantul de sub capsula
        for (float x = -1f; x <= 1f; x += 1f) 
        {
            for (float z = -1f; z <= 1f; z += 1f)
            {            
                Vector3 offset = new Vector3(x, 0, z).normalized * capsule.radius;
                ray.origin = transform.position + offset + Vector3.up * groundedThreshold;
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {
                    grounded = true;
                    break;
                }
            }
        }
        if (grounded)
        {//sare doar daca e pe pamant
            animator.SetBool("Midair", false);
            if (Input.GetButtonUp("Jump"))
            {
                Vector3 jumpForce = (Vector3.up + moveDir).normalized * jumpPower;
                rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
            }
        }
        else
        {//in aer
            animator.SetBool("Midair", true);
        }
    }

    private void OnAnimatorMove()
    {
        if (animator.GetBool("Midair"))
            return;//daca e in aer, lasa motorul de fizica sa gestioneze viteza, deci nu mai executa codu de mai jos
        //transform.position += dir * playerSpeed * Time.deltaTime; //suprascriere fortata de pozitie, nu se foloseste cu rigidbody
        float velY = rigidbody.velocity.y; // retinem viteza pe verticala
        rigidbody.velocity = animator.deltaPosition / Time.deltaTime; //suprascriem viteza cu controlul
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, // pastram componenta verticala calcualta de motorul de fizica
                                         rigidbody.velocity.z);
    }
    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 0.001 || !stateInfo.IsName("Grounded"))
            return; //rotim doar daca se misca si daca nu e in aer
        Quaternion newRotation = Quaternion.LookRotation(moveDir); // noua rotatie, ce se uita in directia de miscare
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotSpeed);
    }
    private void GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal"); //-1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W, 0 altfel
        //vectorul directie de miscare, in spatiul lume, relativ la orientarea camerei:
        moveDir = Vector3.Scale(h * cameraTransform.right +
                                v * cameraTransform.forward,
                                new Vector3(1, 0, 1)).normalized;
    }
}
