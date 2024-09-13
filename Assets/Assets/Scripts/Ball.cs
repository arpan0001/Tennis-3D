using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    private Rigidbody rb;

    private void Start()
    {
        initialPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(ResetPositionAfterDelay(1f));
        }
    }

    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = initialPos;
    }
}