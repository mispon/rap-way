using UnityEngine;

public class ElementTitleAttribute: PropertyAttribute
{
    public string draw_name { get; }

    public ElementTitleAttribute(string _name)
    {
        draw_name = _name;
    }
}

