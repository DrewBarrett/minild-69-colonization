﻿using UnityEngine;
using System.Collections;

public class EnemyHealth : Health {
    public GameObject dropPrefab;
    protected override void Die()
    {
        base.Die();
        //GetComponent<BoxCollider2D>().enabled = false;
        //transform.Rotate(0, 0, 90);
        GetComponent<Rigidbody2D>().isKinematic = false;
        Instantiate(dropPrefab, new Vector3(transform.position.x, 0), Quaternion.identity);
    }
}