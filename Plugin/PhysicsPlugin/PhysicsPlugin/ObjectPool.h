#ifndef OBJECT_POOL_H
#define OBJECT_POOL_H

#include "Particle.h"

class ParticlePool
{
	public:
		ParticlePool(int size);
		~ParticlePool();

		void AddItem(Particle* part);
		void RemoveItem(Particle* part);
	private:
		Particle* particles;
		Particle* nextAvailable;
		int nextAvailableIndex;
		int maxSize;
};

#endif
