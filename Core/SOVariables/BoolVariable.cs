using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    public class BoolVariable : ScriptableObject
    {
        public bool Value;

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariable value)
        {
            Value = value.Value;
        }
    }
}