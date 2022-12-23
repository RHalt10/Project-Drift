using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that moves a projectile at a given speed
 * Written by Nikhil Ghosh '24
 */

namespace WSoft.Combat
{
    public class ProjectileMovement2D : MonoBehaviour
    {
        public float speed;

        [HideInInspector]
        public Vector2 direction;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float movementDist = speed * Time.deltaTime;
            transform.Translate(direction.normalized * movementDist);
        }
    }
}