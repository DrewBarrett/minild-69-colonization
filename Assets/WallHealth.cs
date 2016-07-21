using UnityEngine;
using System.Collections;

public class WallHealth : Health {

    public override void SetHealth()
    {
        base.SetHealth();
        GetComponent<SpriteRenderer>().color = Color.green;
    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        float percent = health / maxhealth;
        Vector4 red = new Vector4(1 - percent, percent,0,1);//not actually red but who has time for refactoring
        Color newcolor = red;
        //Debug.Log(newcolor.ToString() + " "  + red + ", " + percent);
        GetComponent<SpriteRenderer>().color = newcolor;
    }
}
