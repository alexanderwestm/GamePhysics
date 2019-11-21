#ifndef PARTICLE_H
#define PARTICLE_H

#include "Vector3.h"

class Particle
{
	public:
		int UID;
		Particle();

		Particle* getNext() const { return nextParticle; }
		void setNext(Particle* next) { nextParticle = next; }
	private:
		Particle* nextParticle;

		float mass;
		Vector3 position;
		Vector3 velocity;
		Vector3 acceleration;


};

#endif // !PARTICLE_H