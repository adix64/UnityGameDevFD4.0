using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Transform enemiesContainer;
    public Transform weapon;
    public Transform weaponTip;
    List<Transform> enemies;
    Transform enemy;
    Transform head;
    Transform chest;
    Transform spine;
    Transform rightHand;
    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        chest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        InitEnemies();
    }

    private void InitEnemies()
    {
        enemies = new List<Transform>();
        for (int i = 0; i < enemiesContainer.childCount; i++)
            enemies.Add(enemiesContainer.GetChild(i));
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
        HandleDeath();
    }
    void HandleDeath()
    {
        if (transform.position.y < -40f || stateInfo.IsName("Die") && stateInfo.normalizedTime > .99f)
        {
            SceneManager.LoadScene("Basics");
        }
    }
    void HandleWeaponBehaviour()
    {
        bool aiming = Input.GetButton("Fire2");
        animator.SetBool("Aiming", aiming);
        if (aiming)
        {
            animator.SetLayerWeight(1, 1f); // animeaza mainile cu influenta maxima a layerului ArmsAndTorso
            weapon.gameObject.SetActive(true); // seteaza arma vizibila cand tinteste
            CopyRightHandTransformOnWeapon();
            head.rotation = Quaternion.LookRotation(cameraTransform.forward);
            if (Vector3.Dot(weaponTip.right, cameraTransform.forward) < .9f)
            {//daca arma nu se uita cum trebuie, fa rotatia ce aliniaza directia armei cu directia de privire a camerei
                Quaternion alignWeaponToCamRot = Quaternion.FromToRotation(weaponTip.right, cameraTransform.forward);
                alignWeaponToCamRot.ToAngleAxis(out float angle, out Vector3 axis);
                chest.rotation = Quaternion.AngleAxis(angle * .9f, axis) * chest.rotation;
            }
            CopyRightHandTransformOnWeapon();

            if (Input.GetButtonDown("Fire1"))
            {//instantiaza proiectilul la pozitia varfului armei
                var go = GameObject.Instantiate(projectilePrefab);
                go.transform.position = weaponTip.position;
                go.transform.rotation = weaponTip.rotation;
            }
        }
        else
        {// seteaza arma invizibila cand nu tinteste
            weapon.gameObject.SetActive(false);
        }
    }

    private void CopyRightHandTransformOnWeapon()
    {
        weapon.transform.position = rightHand.position;
        weapon.transform.rotation = rightHand.rotation;
    }

    private void LateUpdate()
    {
        // suprascriere de transform-uri ale oaselor, se face doar in LateUpdate
      
        if (enemy != null && !stateInfo.IsTag("takeHit")) //uite-te la enemy
            head.LookAt(enemy.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.UpperChest));
        
        HandleWeaponBehaviour();
    }
    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && !animator.GetBool("Aiming"))
            animator.SetTrigger("Attack");
    }
    private void UpdateAnimatorParameters()
    {//da parametrii de miscare animatorului, in spatiul  local al personajului
        Vector3 characterSpaceDir = transform.InverseTransformDirection(moveDir);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.15f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.15f, Time.deltaTime);
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
        Vector3 deltaPosition = animator.deltaPosition.magnitude * moveDir;//ca sa se deplaseze in directia exacta
        rigidbody.velocity = deltaPosition / Time.deltaTime; //suprascriem viteza cu controlul
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, // pastram componenta verticala calcualta de motorul de fizica
                                         rigidbody.velocity.z);
    }
    private Vector3 GetLookDirection()
    {
        Vector3 lookDirection = moveDir;

        float minDistance = float.MaxValue;
        int closestEnemyIndex = -1;
        for (int i = 0; i < enemies.Count; i++)
        {//daca exista la mai putin de 4m un inamic, ia-l cel mai apropiat
            float dist = Vector3.Distance(transform.position, enemies[i].position);
            if (dist < 4f && dist < minDistance)
            {
                minDistance = dist;
                closestEnemyIndex = i;
            }
        }
        if (closestEnemyIndex != -1)
        {
            enemy = enemies[closestEnemyIndex];
            lookDirection = enemy.position - transform.position;
            animator.SetFloat("distToOpponent", lookDirection.magnitude);
        }
        else
        {//nu e niciun inamic la mai putin de 4m
            enemy = null;
            animator.SetFloat("distToOpponent", 5f);
        }
        if (animator.GetBool("Aiming")) // daca tinteste, intoarce personajul catre camera
            lookDirection = cameraTransform.forward;

        return Vector3.Scale(lookDirection, new Vector3(1, 0, 1)).normalized;
    }
    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 0.001 && !animator.GetBool("Aiming") || stateInfo.IsName("Midair"))
            return; //rotim doar daca se misca si daca nu e in aer

        Vector3 lookDirection = GetLookDirection();
        // noua rotatie, ce se uita in directia de miscare sau catre inamic:
        Quaternion newRotation = Quaternion.LookRotation(lookDirection);
        // suprasscriem rotatia actuala, smoothly cu spherical liniar interpolation(SLERP)
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
