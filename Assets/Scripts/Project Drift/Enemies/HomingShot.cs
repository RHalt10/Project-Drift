using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class HomingShot : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public int damage; 
    //public bool hitPlayer = false;

    void Update()
    {
        Vector2 direction = (Vector2)PlayerController.Instance.transform.position - (Vector2)transform.position;
        float movementDist = speed * Time.deltaTime;
        transform.Translate(direction.normalized * movementDist);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(damage);
            Destroy(gameObject); 
        }
    }
}