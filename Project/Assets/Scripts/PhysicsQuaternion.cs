using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PhysicsQuaternion
{
    public float w;
    public float x;
    public float y;
    public float z;

    public static PhysicsQuaternion identity { get; } = new PhysicsQuaternion(1, 0, 0, 0);

    public float length { get { return Mathf.Sqrt(sqLength); } private set { length = value; } }
    public float sqLength { get { return w * w + x * x + y * y + z * z; } private set { length = value; } }

    public PhysicsQuaternion(float x, float y, float z, float w)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public PhysicsQuaternion(Quaternion quaternion)
    {
        w = quaternion.w;
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
    }

    public static PhysicsQuaternion operator*(PhysicsQuaternion quat, Vector3 vec)
    {
        return new PhysicsQuaternion();
    }

    public static PhysicsQuaternion operator*(Vector3 vec, PhysicsQuaternion quat)
    {
        return quat * vec;
    }

    public static PhysicsQuaternion operator*(float scalar, PhysicsQuaternion quat)
    {
        quat.w *= scalar;
        quat.x *= scalar;
        quat.y *= scalar;
        quat.z *= scalar;
        return quat;
    }

    public static PhysicsQuaternion operator*(PhysicsQuaternion quat, float scalar)
    {
        return scalar * quat;
    }

    public static PhysicsQuaternion operator *(PhysicsQuaternion a, PhysicsQuaternion b)
    {
        PhysicsQuaternion returnQuat = new PhysicsQuaternion();
        Vector3 vector = Vector3.zero;
        Vector3 vectorA = new Vector3(a.x, a.y, a.z), vectorB = new Vector3(b.x, b.y, b.z);

        vector = a.w * vectorB + b.w * vectorA + Vector3.Cross(vectorA, vectorB);

        returnQuat.w = a.w * b.w - Vector3.Dot(vectorA, vectorB);
        returnQuat.x = vector.x;
        returnQuat.y = vector.y;
        returnQuat.z = vector.z;

        return returnQuat;
    }

    public static PhysicsQuaternion operator+(PhysicsQuaternion a, PhysicsQuaternion b)
    {
        PhysicsQuaternion returnQuat = new PhysicsQuaternion();
        returnQuat.w = a.w + b.w;
        returnQuat.x = a.x + b.x;
        returnQuat.y = a.y + b.y;
        returnQuat.z = a.z + b.z;
        return returnQuat;
    }

    public void Normalize()
    {
        float invLength = 1 / length;
        w = w * invLength;
        x = x * invLength;
        y = y * invLength;
        z = z * invLength;
    }

    public Quaternion GetQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    public Matrix4x4 ConvertToMatrix()
    {
        Matrix4x4 returnMatrix = new Matrix4x4();

        Vector4 col1, col2, col3, col4;

        // slide 52
        col1 = new Vector4(w * w + x * x - y * y - z * z, 2 * (x * y + w * z), 2 * (x * z - w * y), 0);
        col2 = new Vector4(2 * (x * y - w * z), w * w - x * x + y * y - z * z, 2 * (y * z + w * x), 0);
        col3 = new Vector4(2 * ( x * z + w * y), 2 * (y * z - w * x), w * w - x * x - y * y + z * z, 0);
        col4 = new Vector4(0, 0, 0, 1);

        return new Matrix4x4(col1, col2, col3, col4);
    }
}
