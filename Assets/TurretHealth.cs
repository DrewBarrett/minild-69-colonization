using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretHealth : Health {
    public bool built = false;
    int buildCost = 50;
    internal int upgradeLevel = 0;
    override protected void Start()
    {
        base.Start();
        if (!built)
        {
            Dead = true;
        }
    }
    /*void Update()
    {
        if (upgradeLevel > 0)
        {
            //if we have been built

        }
    }*/
    public override void Build()
    {
        built = true;
        upgradeLevel++;
        //gameObject.tag = "UnTagged";
        SetHealth();
        GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
        GetComponentInChildren<Text>(true).text = "Activate to upgrade turret for 50 coins";
    }
    public override int BuildCost()
    {
        return buildCost;
    }
    public override void SetHealth()
    {
        base.SetHealth();
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    internal void Destroy()
    {
        built = false;
        upgradeLevel = 0;
    }
}
