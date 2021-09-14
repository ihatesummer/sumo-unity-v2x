using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorCollection
{
    public float min { get; set; }
    public float max { get; set; }
    public float average { get; set; }
    public ErrorCollection()
    {
        min = default;
        max = default;
        average = default;
    }
}
