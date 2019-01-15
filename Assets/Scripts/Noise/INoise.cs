using System.Collections.Generic;
using UnityEngine;

public interface INoise
{
    Shader Shader { get; }
    List<string> Property { get; }
    float Threshold { get; }
    void Init();

}