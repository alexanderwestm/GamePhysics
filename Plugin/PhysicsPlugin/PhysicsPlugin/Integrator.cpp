#include "Integrator.h"

void Integrator::UpdatePositionEulerExplicit(Vector3& pos, Vector3& vel, Vector3& acc, float dt)
{
	pos += vel * dt;
	vel += acc * dt;
}

void Integrator::UpdateRotationEulerExplicit(Quaternion& rot, Vector3& vel, Vector3& acc, float dt)
{
	rot += rot * vel * .5f * dt;
	rot.normalize();
	vel += acc * dt;
}

void Integrator::UpdatePositionKinematic(Vector3& pos, Vector3& vel, Vector3& acc, float dt)
{
	pos += vel * dt + acc * .5f * dt * dt;
	vel += acc * dt;
}

void Integrator::UpdateRotationKinematic(Quaternion& rot, Vector3& vel, Vector3& acc, float dt)
{
	//rot += vel * rot * dt * .5f + acc * rotation * dt * dt * .25f - rotation * angularVelocity.sqrMagnitude * dt * dt * .125f;
	rot += rot * vel * dt * .5f + rot * acc * dt * dt * .25f - rot * vel.sqLength * dt * dt * .125f;
	rot.normalize();
	vel += acc * dt;
}
