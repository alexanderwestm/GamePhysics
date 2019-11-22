#ifndef OBJECT_POOL_H
#define OBJECT_POOL_H

#include "Particle.h"

class ParticlePool
{
	public:
		ParticlePool();
		ParticlePool(int size);
		~ParticlePool();

		int AddItem(Particle* part);
		void RemoveItem(Particle* part);
		inline Particle& GetItem(int index) { return particles[index]; }

		inline int getSize() { return maxSize; }
	private:
		Particle* particles;
		Particle* nextAvailable;
		int nextAvailableIndex;
		int maxSize;
};


#endif
