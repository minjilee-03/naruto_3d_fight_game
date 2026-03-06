using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    [Header("Opponent Movement")]
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;
    public CharacterController characterController;
    public Animator animator;

    [Header("Opponent Fight")]
    public float attackCooldown = 2.0f;
    public int attackDamage = 5;
    public string[] attackAnimations = { "Attack1Animation", "Attack2Animation", "Attack3Animation", "Attack4Animation" };
    public float attackRadius = 2f;
    public int attackCount = 4;
    public int randomNumber;

    [Header("Target Settings")]
    public faghtingController[] fightingController;
    public Transform[] players;

    public bool isTakingDamage = false;
    private float lastAttackTime;

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
        createRandomNumber();
    }

    void Update()
    {
        for (int i = 0; i < fightingController.Length; i++)
        {
            if (players[i] == null || !players[i].gameObject.activeSelf) continue;

            float distance = Vector3.Distance(transform.position, players[i].position);

            // 1. 공격 사거리 안
            if (distance <= attackRadius)
            {
                animator.SetBool("Walking", false);

                // 공격 쿨타임 확인 (피격 중이 아닐 때만 공격)
                if (Time.time - lastAttackTime > attackCooldown && !isTakingDamage)
                {
                    int randomAttackIndex = Random.Range(0, attackAnimations.Length);
                    PerformAttack(randomAttackIndex);

                    // 공격 시점에 나루토의 피격 코루틴 호출
                    fightingController[i].StartCoroutine(fightingController[i].PlayHitDamageAnimation(attackDamage));
                }
            }
            // 2. 공격 사거리 밖 (추적)
            else
            {
                if (!isTakingDamage) // 맞고 있을 때는 이동 불가
                {
                    Vector3 direction = (players[i].position - transform.position).normalized;
                    direction.y = 0;

                    characterController.Move(direction * movementSpeed * Time.deltaTime);

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }

                    animator.SetBool("Walking", true);
                }
            }
        }
    }

    void PerformAttack(int attackIndex)
    {
        animator.Play(attackAnimations[attackIndex]);
        lastAttackTime = Time.time;
    }

    void createRandomNumber()
    {
        randomNumber = Random.Range(1, attackCount + 1);
    }

    // [수정] 사스케가 맞았을 때 실행되는 함수 (소리 즉시 재생)
    public IEnumerator PlayHitDamageAnimation(int takeDamage)
    {
        if (isTakingDamage) yield break;

        isTakingDamage = true;

        // 1. 소리 즉시 재생
        if (hitSounds != null && hitSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(hitSounds[Random.Range(0, hitSounds.Length)], transform.position);
        }

        // 2. 아주 짧은 대기 후 애니메이션과 데미지 처리
        yield return new WaitForSeconds(0.05f);

        currentHealth -= takeDamage;
        healthBar.SetHealth(currentHealth);

        animator.Play("HitDamageAnimation");

        if (currentHealth <= 0) Die();

        // 3. 애니메이션이 끝날 때까지 대기 (맞는 도중 행동 불가)
        yield return new WaitForSeconds(0.8f);
        isTakingDamage = false;
    }

    void Die() { Debug.Log("사스케 사망"); }

    public void Attack1Effect() { if (attack1Effect) attack1Effect.Play(); }
    public void Attack2Effect() { if (attack2Effect) attack2Effect.Play(); }
    public void Attack3Effect() { if (attack3Effect) attack3Effect.Play(); }
    public void Attack4Effect() { if (attack4Effect) attack4Effect.Play(); }
}