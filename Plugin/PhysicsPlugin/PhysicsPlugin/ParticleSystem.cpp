#include "ParticleSystem.h"

ParticleSystem::ParticleSystem(int size)
{
	pool = new ParticlePool(size);
}

void ParticleSystem::updateParticles(float dt)
{
	int size = pool->getSize();
	for (int i = size - 1; i >= 0; --i)
	{
		pool->GetItem(i).update(dt);
	}
}

int ParticleSystem::createParticle()
{
	Particle* part = new Particle();
	part->UID = pool->AddItem(part);
	return part->UID;
}

int ParticleSystem::createParticle(Vector3 pos, Quaternion rot)
{
	Particle* part = new Particle(pos, rot);
	part->UID = pool->AddItem(part);
	return part->UID;
}

void ParticleSystem::setParticleVel(int uid, Vector3 vel)
{
	pool->GetItem(uid).setVelocity(vel);
}

void ParticleSystem::setParticleAngularVel(int uid, Vector3 vel)
{
	pool->GetItem(uid).setAngularVel(vel);
}