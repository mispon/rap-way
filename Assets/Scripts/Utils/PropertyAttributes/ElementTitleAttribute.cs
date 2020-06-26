
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementTitleAttribute: PropertyAttribute
{
    public string draw_name { get; private set; }

    public ElementTitleAttribute(string _name)
    {
        draw_name = _name;
    }
}

