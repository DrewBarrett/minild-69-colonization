using UnityEngine;
using System.Collections;

public class EnemyHealth : Health {
    public GameObject dropPrefab;
    protected override void Die(bool die)
    {
        base.Die(die);
        //GetComponent<BoxCollider2D>().enabled = false;
        //transform.Rotate(0, 0, 90);
        GetComponent<Rigidbody2D>().isKinematic = !die;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        Instantiate(dropPrefab, new Vector3(transform.position.x, 0), Quaternion.identity);
    }
}
