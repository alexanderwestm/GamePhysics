// Fill out your copyright notice in the Description page of Project Settings.


#include "Particle2D.h"
#include "Runtime/Engine/Classes/Engine/World.h"
#include "ForceGenerator.h"
#include "ConstructorHelpers.h"

// Sets default values
AParticle2D::AParticle2D()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AParticle2D::BeginPlay()
{
	Super::BeginPlay();
	position = GetActorLocation();
	SetMass(startMass);
	forceOfGravity = ForceGenerator::GenerateForce_gravity(FVector::UpVector, 9.81, mass);
}

// Called every frame
void AParticle2D::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	SetActorLocation(position);
	SetActorRotation(FRotator(rotation, 0, 0));

	if (particleTickType == TickType::EULER)
	{
		UpdatePositionEulerExplicit(DeltaTime);
		UpdateRotationEulerExplicit(DeltaTime);
	}
	else if (particleTickType == TickType::KINEMATIC)
	{
		UpdatePositionKinematic(DeltaTime);
		UpdateRotationKinematic(DeltaTime);
	}

	// TO DO: replace with surface normal instead of hard coded
	FVector normalForce = ForceGenerator::GenerateForce_normal(forceOfGravity, FVector::UpVector);

	AddForce(forceOfGravity);

	//AddForce(ForceGenerator::GenerateForce_sliding(forceOfGravity, normalForce));

	//AddForce(FVector(1, 0, 0));
	//AddForce(ForceGenerator::GenerateForce_friction_static(normalForce, FVector(10, 0, 0), .5));
	
	//AddForce(FVector(10, 0, 0));
	//AddForce(ForceGenerator::GenerateForce_friction_kinetic(normalForce, velocity, .5));

	//AddForce(forceOfGravity);
	//AddForce(normalForce);

	//AddForce(FVector(.1, 0, 0));
	AddForce(ForceGenerator::GenerateForce_drag(velocity, FVector(0, 0, 0), 0.001225, 1.0, 1.05));

	UE_LOG(LogTemp, Warning, TEXT("Sum Force: %s"), *force.ToString());

	UpdateAcceleration();
}

void AParticle2D::UpdatePositionEulerExplicit(float dt)
{
	position += velocity * dt;
	// something missing here? idr
	velocity += acceleration * dt;
}

void AParticle2D::UpdatePositionKinematic(float dt)
{
	position += velocity * dt + .5 * acceleration * dt * dt;
	// something missing here? idr
	velocity += acceleration * dt;
}

void AParticle2D::UpdateRotationEulerExplicit(float dt)
{
	rotation += angularVelocity * dt;
	// something missing here? idr
	angularVelocity += angularAcceleration * dt;
}

void AParticle2D::UpdateRotationKinematic(float dt)
{
	rotation += angularVelocity * dt + .5 * angularAcceleration * dt * dt;
	angularVelocity += angularAcceleration * dt;
}

void AParticle2D::SetMass(float newMass)
{
	mass = newMass > 0 ? newMass : 0;
	massInv = mass > 0 ? 1 / mass : 0;
}

float AParticle2D::GetMass()
{
	return mass;
}

void AParticle2D::AddForce(FVector newForce)
{
	force += newForce;
}

void AParticle2D::UpdateAcceleration()
{
	acceleration = massInv * force * 100; // convert from cm to m
	force = FVector::ZeroVector;
}