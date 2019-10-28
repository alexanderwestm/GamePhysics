using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void Matrix3x3From4x4(this Matrix4x4 matrix)
    {
        matrix.SetColumn(3, new Vector4(0, 0, 0, 1));
        matrix.SetRow(3, new Vector4(0, 0, 0, 1));
    }
}
