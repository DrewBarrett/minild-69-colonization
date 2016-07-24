using UnityEngine;
using System.Collections;

public class WallHealth : Health
{
    public bool shouldRepair = true;
    public float repairCooldown = 2f;
    public float repairAmount = 1f;
    float cooldownTimer = 0f;

    public void Update()
    {
        if (shouldRepair)
        {
            if (cooldownTimer <= 0)
            {
                AddHealth(repairAmount);
                cooldownTimer = repairCooldown;
            }
            else
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
        else
        {
            cooldownTimer = repairCooldown;
        }
    }

    public override void SetHealth()
    {
        base.SetHealth();
        GetComponent<SpriteRenderer>().color = Color.green;
    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        CalculateColor();
        //tell base that we took damage so we shouldnt repair anymore
        GetComponentInParent<Base>().TakeDamage();
    }
    public override void AddHealth(float amount)
    {
        base.AddHealth(amount);
        CalculateColor();
    }
    public void CalculateColor()
    {
        float percent = health / maxhealth;
        Vector4 red = new Vector4(1 - percent, percent, 0, 1);//not actually red but who has time for refactoring
        Color newcolor = red;
        //Debug.Log(newcolor.ToString() + " "  + red + ", " + percent);
        GetComponent<SpriteRenderer>().color = newcolor;
    }
}
