using UnityEngine;

public class PlayerNeeds : MonoBehaviour
{
    [Header("Need Levels")]
    public float hunger = 100f;
    public float thirst = 100f;
    public float oxygen = 100f;
    public float health = 100f;

    [Header("Decrease Rates per second")]
    public float hungerRate = 1f;
    public float thirstRate = 1f;
    public float oxygenRate = 5f;
    public float damageRate = 10f;

    public bool isInSpace = false;

    void Update()
    {
        hunger = Mathf.Max(0, hunger - hungerRate * Time.deltaTime);
        thirst = Mathf.Max(0, thirst - thirstRate * Time.deltaTime);


        if (isInSpace)
        {
            oxygen = Mathf.Max(0, oxygen - oxygenRate * Time.deltaTime);
        }
        else
        {
            oxygen = Mathf.Min(100, oxygen + (oxygenRate * Time.deltaTime));
        }

        if (hunger <= 0 || thirst <= 0 || oxygen <= 0)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        Debug.Log("Player is taking damage");

        health -= damageRate * Time.deltaTime;
        health = Mathf.Clamp(health, 0, 100);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        transform.position = Vector3.zero;
        health = 100f;
        hunger = 100f;
        thirst = 100f;
        oxygen = 100f;
    }

    public void Eat(float amount)
    {
        hunger = Mathf.Min(100, hunger + amount);
        health = Mathf.Min(100, health + 20f);
    }

    public void Drink(float amount)
    {
        thirst = Mathf.Min(100, thirst + amount);
        health = Mathf.Min(100, health + 10f);
    }

    public void RefillOxygen(float amount)
    {
        oxygen = Mathf.Min(100, oxygen + amount);
    }
    void OnTriggerEnter(Collider other)
    {
    if (other.CompareTag("Ship"))
    {
        isInSpace = false;
    }
}

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ship"))
            {
                isInSpace = true;
            }
        }

}
