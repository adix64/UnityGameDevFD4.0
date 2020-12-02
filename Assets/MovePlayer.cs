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

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GetMoveDirection();
        ApplyRootMotion();
        ApplyRootRotation();
        HandleJump();
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
            if (Input.GetButtonUp("Jump"))
                rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
    }

    private void ApplyRootMotion()
    {
        //transform.position += dir * playerSpeed * Time.deltaTime;
        float velY = rigidbody.velocity.y; // retinem viteza pe verticala
        rigidbody.velocity = moveDir * playerSpeed; //suprascriem viteza cu controlul
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, // pastram componenta verticala calcualta de motorul de fizica
                                         rigidbody.velocity.z);
    }
    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 0.001f)//nu se misca, deci nu roti
            return;
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
