// Fill out your copyright notice in the Description page of Project Settings.


#include "Particle2D.h"
#include "Runtime/Engine/Classes/Engine/World.h"

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
	UE_LOG(LogTemp, Warning, TEXT("Default Position: %s"), *position.ToString())
}

// Called every frame
void AParticle2D::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	acceleration.X = (float)sin(GetWorld()->GetRealTimeSeconds()) * 10;
	UpdatePositionEulerExplicit(DeltaTime);
	
	UE_LOG(LogTemp, Warning, TEXT("Acceleration After Update: %s"), *acceleration.ToString())
	//UpdateRotationEulerExplicit(DeltaTime);

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