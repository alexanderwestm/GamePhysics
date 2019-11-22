#include "ParticlePool.h"

#include <assert.h>

ParticlePool::ParticlePool()
{
}

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

int ParticlePool::AddItem(Particle* part)
{
	assert(nextAvailable != nullptr);

	if (nextAvailableIndex > 0)
	{
		particles[nextAvailableIndex - 1].setNext(part);
	}
	part->setNext(nextAvailable->getNext());
	nextAvailable = nextAvailable->getNext();
	int num = nextAvailableIndex;
	nextAvailableIndex++;
	return num;
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