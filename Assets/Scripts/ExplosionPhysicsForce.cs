using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosionPhysicsForce : MonoBehaviour
{
    public float explosionForce = 4;
    public float r = 10;
    public float damage;
    public PlayerController owner;

    void Update()
    {
        Debug.DrawLine(transform.position, new Vector3(transform.position.x + 10, transform.position.y, transform.position.z));
    }

    private IEnumerator Start()
    {
        // wait one frame because some explosions instantiate debris which should then
        // be pushed by physics force
        yield return null;

        var cols = Physics.OverlapSphere(transform.position, r);
        var rigidbodies = new List<Rigidbody>();
        AudioManager.instance.PlaySfx("Explosion");
        foreach (var col in cols)
        {
            if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
            {
                rigidbodies.Add(col.attachedRigidbody);
            }

            if(col.gameObject.GetComponent<PlayerController>() != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                col.gameObject.GetComponent<PlayerController>().TakeDamage(damage / dist,owner);
            }
        }
        foreach (var rb in rigidbodies)
        {
            rb.AddExplosionForce(explosionForce, transform.position, r, 1, ForceMode.Impulse);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}