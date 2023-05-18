using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ignite", menuName = "Status Effects [Don't Use]/Ignite")]
public class Ignite : StatusEffect
{
    [SerializeField]
    float dps = 1f;
    float timeToDamage;

    int stacks = 1;
    public override int Stacks
    {
        protected set
        {
            dps *= (float)value / stacks;
            timeToDamage = 1f / dps;
            stacks = value;
        }
        get { return stacks; }
    }

    private void Awake()
    {
        timeToDamage = 1f / dps;
    }

    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Ignite instance = (Ignite)base.Instantiate(gameObject);

        instance.Stacks = Stacks;
        instance.Duration = Duration;
        instance.dps = dps;

        return instance;
    }


    // Update is called once per frame
    internal override void Update()
    {
        base.Update();
        timeToDamage -= Time.deltaTime;
        if (timeToDamage <= 0)
        {
            gameObject.GetComponent<Health>().ReceiveAttack(new Attack(1, this));
            timeToDamage += 1f / dps;
        }
    }
}
