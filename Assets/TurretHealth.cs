using UnityEngine;
using System.Collections;

public class TurretHealth : Health {
    public bool built = false;
    int buildCost = 50;
    override protected void Start()
    {
        base.Start();
        if (!built)
        {
            Dead = true;
        }
    }
    public override void Build()
    {
        built = true;
        gameObject.tag = "UnTagged";
        SetHealth();
        GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
    }
    public override int BuildCost()
    {
        return buildCost;
    }
    public override void SetHealth()
    {
        base.SetHealth();
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
