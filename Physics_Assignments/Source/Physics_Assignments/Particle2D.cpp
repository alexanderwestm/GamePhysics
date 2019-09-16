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

	UE_LOG(LogTemp, Warning, TEXT("Gravity Force: %s"), *forceOfGravity.ToString())


	AddForce(ForceGenerator::GenerateForce_normal(forceOfGravity, FVector::UpVector));

	UE_LOG(LogTemp, Warning, TEXT("Normal Force: %s"), *force.ToString())

	UpdateAcceleration();

	SetActorLocation(position);
	SetActorRotation(FRotator(rotation, 0, 0));
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