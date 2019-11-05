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

    public static void Vector3Clamp(this Vector3 vector, Vector3 min, Vector3 max)
    {
        vector.x = Mathf.Clamp(vector.x, min.x, max.x);
        vector.y = Mathf.Clamp(vector.y, min.y, max.y);
        vector.z = Mathf.Clamp(vector.z, min.z, max.z);
    }
}
