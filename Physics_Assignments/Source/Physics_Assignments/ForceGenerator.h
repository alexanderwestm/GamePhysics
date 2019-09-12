// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * 
 */
class PHYSICS_ASSIGNMENTS_API ForceGenerator
{
public:
	ForceGenerator();
	~ForceGenerator();

	
	FVector GenerateForce_gravity(FVector worldUp, float gravitationalConstant, float particleMass);
	FVector GenerateForce_normal(FVector f_gravity, FVector surfaceNormal_unit);
	FVector GenerateForce_sliding(FVector f_gravity, FVector f_normal);
	FVector GenerateForce_friction_static(FVector f_normal, FVector f_opposing, float frictionCoefficient_static);
	FVector GenerateForce_friction_kinetic(FVector f_normal, FVector particleVelocity, float frictionCoefficient_kinetic);
	FVector GenerateForce_drag(FVector particleVelocity, FVector fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient);
	FVector GenerateForce_spring(FVector particlePosition, FVector anchorPosition, float springRestingLength, float springStiffnessCoefficient);
};
