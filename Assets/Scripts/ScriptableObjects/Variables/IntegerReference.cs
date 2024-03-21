using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntegerReference 
{
    public IntegerVariable Variable;
    public int Value
    {
        get { return Variable.Value; }
        set { Variable.Value = value; }
    }
}
