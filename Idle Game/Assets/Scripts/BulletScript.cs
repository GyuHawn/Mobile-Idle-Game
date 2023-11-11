using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float delay;
    public GameObject target;

    private void Start()
    {
        delay = 3f;
    }

    private void Update()
    {
        if (target == null) 
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, delay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("MiniGame"))
        {
            Destroy(gameObject);
        }
    }
}