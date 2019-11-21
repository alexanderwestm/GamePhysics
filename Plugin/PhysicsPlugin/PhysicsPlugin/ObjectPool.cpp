#include "ObjectPool.h"

#include <assert.h>

ParticlePool::ParticlePool(int size)
{
	particles = new Particle[size];
	nextAvailableIndex = 0;
	maxSize = size;
	nextAvailable = &particles[0];
	for (int i = 0; i < maxSize; ++i)
	{
		particles[i].setNext(&particles[i + 1]);
	}

	particles[maxSize - 1].setNext(nullptr);
}

ParticlePool::~ParticlePool()
{
	delete[] particles;
}

void ParticlePool::AddItem(Particle* part)
{
	assert(nextAvailable != nullptr);

	if (nextAvailableIndex > 0)
	{
		particles[nextAvailableIndex - 1].setNext(part);
	}
	part->setNext(nextAvailable->getNext());
	nextAvailable = nextAvailable->getNext();
	nextAvailableIndex++;
}

void ParticlePool::RemoveItem(Particle* part)
{
	for (int i = 0; i < maxSize; ++i)
	{
		if (part->UID == particles[i].UID)
		{
			nextAvailableIndex = i;
			nextAvailable = &particles[i];
		}
	}
}