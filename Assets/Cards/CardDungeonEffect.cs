using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CardDungeonEffect : MonoBehaviour
{
    public string description = "";
    public virtual string GetFormatedDescription()
    {
        return description;
    }
}
