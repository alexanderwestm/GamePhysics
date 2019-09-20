// Fill out your copyright notice in the Description page of Project Settings.


#include "ForceGenerator.h"
#include "GenericPlatformMath.h"

// works
FVector ForceGenerator::GenerateForce_gravity(FVector worldUp, float gravitationalConstant, float particleMass)
{
	// f_gravity = mg
	return worldUp * -gravitationalConstant * particleMass;
}

// works
FVector ForceGenerator::GenerateForce_normal(FVector f_gravity, FVector surfaceNormal_unit)
{
	// f_normal = proj(f_gravity, surfaceNormal_unit)

	FVector u = surfaceNormal_unit, v = f_gravity;
	FVector returnVec = f_gravity.ProjectOnTo(surfaceNormal_unit);

	return -returnVec;

	//return f_gravity.ProjectOnTo(surfaceNormal_unit);
}

// works
FVector ForceGenerator::GenerateForce_sliding(FVector f_gravity, FVector f_normal)
{
	// f_sliding = f_gravity + f_normal
	return f_gravity + f_normal;
}

FVector ForceGenerator::GenerateForce_friction_static(FVector f_normal, FVector f_opposing, float frictionCoefficient_static)
{
	// f_friction_s = -f_opposing if less than max, else -coeff*f_normal (max amount is coeff*|f_normal|)
	float max = frictionCoefficient_static * f_normal.Size();

	if (f_opposing.Size() < max)
	{
		return -f_opposing;
	}
	else
	{
		return -max * f_opposing / f_opposing.Size();
	}
}

FVector ForceGenerator::GenerateForce_friction_kinetic(FVector f_normal, FVector particleVelocity, float frictionCoefficient_kinetic)
{

	// f_friction_k = -coeff*|f_normal| * unit(vel)
	//FVector absNormal = FVector(Math.Abs(f_normal.X), f_normal.Y, f_normal.Z);
	FVector normalizeVelocity  = particleVelocity;
	normalizeVelocity.Normalize();
	return -frictionCoefficient_kinetic * f_normal.Size() * normalizeVelocity;
}

FVector ForceGenerator::GenerateForce_drag(FVector particleVelocity, FVector fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
{
	// f_drag = (p * u^2 * area * coeff)/2
	//return fluidDensity * sumVelocity^2 * objectArea * coeff
	FVector sumVel = fluidVelocity - particleVelocity;
	return (fluidDensity * sumVel.Size() * sumVel * objectArea_crossSection * objectDragCoefficient) * .5;
}

FVector ForceGenerator::GenerateForce_spring(FVector particlePosition, FVector anchorPosition, float springRestingLength, float springStiffnessCoefficient)
{
	// f_spring = -coeff*(spring length - spring resting length)
	FVector forceDir = particlePosition - anchorPosition;
	float springLength = forceDir.Size();
	return springStiffnessCoefficient * (springRestingLength - springLength) * forceDir / springLength;
}