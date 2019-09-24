using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ForceGenerator
{
    public static Vector2 GenerateForce_gravity(Vector2 worldUp, float gravitationalConstant, float particleMass)
    {
        // f_gravity = mg
        return worldUp * -gravitationalConstant * particleMass;
    }

    public static Vector2 GenerateForce_normal(Vector2 f_gravity, Vector2 surfaceNormal_unit)
    {
        // f_normal = proj(f_gravity, surfaceNormal_unit)

        return -Vector3.Project(f_gravity, surfaceNormal_unit); ;
    }

    public static Vector2 GenerateForce_sliding(Vector2 f_gravity, Vector2 f_normal)
    {
        // f_sliding = f_gravity + f_normal
        return f_gravity + f_normal;
    }

    public static Vector2 GenerateForce_friction_static(Vector2 f_normal, Vector2 f_opposing, float frictionCoefficient_static)
    {
        // f_friction_s = -f_opposing if less than max, else -coeff*f_normal (max amount is coeff*|f_normal|)
        float max = frictionCoefficient_static * f_normal.magnitude;

        if (f_opposing.magnitude < max)
        {
            return -f_opposing;
        }
        else
        {
            return -max * f_opposing / f_opposing.magnitude;
        }
    }

    public static Vector2 GenerateForce_friction_kinetic(Vector2 f_normal, Vector2 particleVelocity, float frictionCoefficient_kinetic)
    {

        // f_friction_k = -coeff*|f_normal| * unit(vel)
        //Vector2 absNormal = Vector2(Math.Abs(f_normal.X), f_normal.Y, f_normal.Z);
        Vector2 normalizeVelocity = particleVelocity;
        normalizeVelocity.Normalize();
        return -frictionCoefficient_kinetic * f_normal.magnitude * normalizeVelocity;
    }

    public static Vector2 GenerateForce_drag(Vector2 particleVelocity, Vector2 fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        // f_drag = (p * u^2 * area * coeff)/2
        //return fluidDensity * sumVelocity^2 * objectArea * coeff
        Vector2 sumVel = fluidVelocity - particleVelocity;
        return (fluidDensity * sumVel.magnitude * sumVel * objectArea_crossSection * objectDragCoefficient) * .5f;
    }

    public static Vector2 GenerateForce_spring(Vector2 particlePosition, Vector2 anchorPosition, float springRestingLength, float springStiffnessCoefficient)
    {
        // f_spring = -coeff*(spring length - spring resting length)
        Vector2 forceDir = particlePosition - anchorPosition;
        float springLength = forceDir.magnitude;
        return springStiffnessCoefficient * (springRestingLength - springLength) * forceDir / springLength;
    }
}
