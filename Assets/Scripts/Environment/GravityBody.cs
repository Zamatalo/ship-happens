using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        Asteroid closest = GetClosestAsteroid();
        if (closest == null || !closest.IsWithinGravityField(transform.position))
            return;

        Vector3 gravity = closest.GetGravity(transform.position);
        rb.AddForce(gravity);

        // Вращаем объект, чтобы "низ" смотрел на центр астероида
        Vector3 gravityUp = -closest.GetGravityDirection(transform.position);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
    }

    Asteroid GetClosestAsteroid()
    {
        Asteroid closest = null;
        float minDist = float.MaxValue;

        foreach (Asteroid a in Asteroid.All)
        {
            float dist = Vector3.Distance(transform.position, a.transform.position);
            if (dist < minDist)
            {
                closest = a;
                minDist = dist;
            }
        }

        return closest;
    }
}