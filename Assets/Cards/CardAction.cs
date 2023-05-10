using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    abstract public class CardAction : MonoBehaviour
    {
        public string description = "";
        public virtual string GetFormatedDescription()
        {
            return description;
        }

        public abstract void PreviewEffect();
        public abstract void CancelPreview();
        public abstract void ConfirmPreview();
    }
}