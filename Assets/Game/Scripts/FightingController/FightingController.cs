using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faghtingController : MonoBehaviour
{
    [Header("Player Movement")]
    public float movementSpeed = 3f;
    public float rotationSpeed = 10f;
    private CharacterController characterController;
    private Animator animator;

    [Header("Player Fight")]
    public float attackCooldown = 0.5f;
    public int attackDamage = 5;
    public string[] attackAnimations = { "Attack1Animation", "Attack2Animation", "Attack3Animation", "Attack4Animation" };
    public float dodgeDistance = 5f; 
    public float attackRadius = 2.2f;
    public Transform[] opponents;
    private float lastAttackTime;
    private bool isTakingDamage = false;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("Effects and sounds")]
    public ParticleSystem attack1Effect;
    public ParticleSystem attack2Effect;
    public ParticleSystem attack3Effect;
    public ParticleSystem attack4Effect;
    public AudioClip[] hitSounds;

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.GiveFullHealth(currentHealth);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformDodgeFront();
            return; 
        }

        if (isTakingDamage) return;

        PerformMovement();

        if (Input.GetKeyDown(KeyCode.Alpha1)) PerformAttack(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) PerformAttack(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) PerformAttack(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) PerformAttack(3);
    }

    void PerformMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-v, 0f, h);

        if (move != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        move.y = -9.81f;
        characterController.Move(move * movementSpeed * Time.deltaTime);
    }

    void PerformAttack(int index)
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            animator.Play(attackAnimations[index]);
            lastAttackTime = Time.time;

            foreach (Transform opponent in opponents)
            {
                if (Vector3.Distance(transform.position, opponent.position) <= attackRadius)
                {
                    OpponentAI ai = opponent.GetComponent<OpponentAI>();
                    ai.StartCoroutine(ai.PlayHitDamageAnimation(attackDamage));
                }
            }
        }
    }
    void PerformDodgeFront()
    {
        animator.Play("DodgeFrontAnimation");
        Vector3 dodgeDir = transform.forward * dodgeDistance;
        characterController.Move(dodgeDir * Time.deltaTime * 10f);
    }

    public IEnumerator PlayHitDamageAnimation(int takeDamage)
    {
        if (isTakingDamage) yield break;
        isTakingDamage = true;

        if (hitSounds != null && hitSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(hitSounds[Random.Range(0, hitSounds.Length)], transform.position);
        }

        yield return new WaitForSeconds(0.05f);

        currentHealth -= takeDamage;
        healthBar.SetHealth(currentHealth);

        animator.Play("HitDamageAnimation");

        if (currentHealth <= 0) Die();

        yield return new WaitForSeconds(0.8f);
        isTakingDamage = false;
    }

    void Die() { Debug.Log("나루토 사망"); }

    public void Attack1Effect() { if (attack1Effect) attack1Effect.Play(); }
    public void Attack2Effect() { if (attack2Effect) attack2Effect.Play(); }
    public void Attack3Effect() { if (attack3Effect) attack3Effect.Play(); }
    public void Attack4Effect() { if (attack4Effect) attack4Effect.Play(); }
}