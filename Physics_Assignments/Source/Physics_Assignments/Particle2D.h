// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Particle2D.generated.h"

UENUM()
enum class TickType
{
	EULER = 0,
	KINEMATIC
};

UCLASS()
class PHYSICS_ASSIGNMENTS_API AParticle2D : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AParticle2D();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UPROPERTY(EditAnywhere)
		FVector position;
	UPROPERTY(EditAnywhere)
		FVector velocity;
	UPROPERTY(EditAnywhere)
		FVector acceleration;
	UPROPERTY(EditAnywhere)
		float rotation;
	UPROPERTY(EditAnywhere)
		float angularVelocity;
	UPROPERTY(EditAnywhere)
		float angularAcceleration;
	UPROPERTY(EditAnywhere)
		float startMass;
	UPROPERTY(EditAnywhere)
		TickType particleTickType;

	void SetMass(float newMass);
	float GetMass();

	void AddForce(FVector newForce);
private:

	void UpdatePositionEulerExplicit(float dt);
	void UpdatePositionKinematic(float dt);
	void UpdateRotationEulerExplicit(float dt);
	void UpdateRotationKinematic(float dt);

	void UpdateAcceleration();

	float mass, massInv;
	FVector force;
};
