using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Animator animator;

    public GameObject Explode;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }

    }
    void Die()
    {
        ScoreScript.scoreValue += 10;
        Debug.Log("Enemy deid!");
        StartCoroutine(secondDeath(0.35f));
    }
    IEnumerator secondDeath(float sec)
    {
        yield return new WaitForSeconds(sec);
        Instantiate(Explode, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
