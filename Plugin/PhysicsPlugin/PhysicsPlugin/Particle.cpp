#include "Particle.h"
#include "Vector3.h"
#include "Integrator.h"

Particle::Particle()
{
	position = Vector3::zero;
	velocity = Vector3::zero;
	acceleration = Vector3::zero;
	
	rotation = Quaternion::identity;
	angularVelocity = Vector3::zero;
	angularAcceleration = Vector3::zero;
}

Particle::Particle(Vector3 pos, Quaternion rot)
{
	position = pos;
	rotation = rot;
}

void Particle::update(float dt)
{
	switch (updateMode)
	{
		case UpdateMode::INVALID:
			break;
		case UpdateMode::KINEMATIC:
			Integrator::UpdatePositionKinematic(position, velocity, acceleration, dt);
			Integrator::UpdateRotationKinematic(rotation, angularVelocity, angularAcceleration, dt);
			break;
		case UpdateMode::EULER:
			Integrator::UpdatePositionEulerExplicit(position, velocity, acceleration, dt);
			Integrator::UpdateRotationEulerExplicit(rotation, angularVelocity, angularAcceleration, dt);
			break;
		default:
			break;
	}
}