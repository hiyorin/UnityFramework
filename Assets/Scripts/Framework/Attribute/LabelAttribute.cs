using UnityEngine;

public class LabelAttribute : PropertyAttribute
{
    public string label;
    public LabelAttribute (string label)
    {
        this.label = label;
    }
}

public class LabelRangeAttribute : LabelAttribute
{
    public float min;
    public float max;
    public LabelRangeAttribute (string label, float min, float max) : base (label)
    {
        this.min = min;
        this.max = max;
    }
}
