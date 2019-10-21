using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PhysicsQuaternion
{
    public float w;
    public float x;
    public float y;
    public float z;
    public PhysicsQuaternion(float x, float y, float z, float w)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
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

    public static PhysicsQuaternion operator *(PhysicsQuaternion quat, PhysicsQuaternion other)
    {
        return new PhysicsQuaternion();
    }

    public static PhysicsQuaternion operator+(PhysicsQuaternion quat, PhysicsQuaternion other)
    {
        return new PhysicsQuaternion();
    }
}
