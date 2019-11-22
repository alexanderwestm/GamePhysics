#ifndef PARTICLE_SYSTEM_H
#define PARTICLE_SYSTEM_H

#include "ParticlePool.h"

class ParticleSystem
{
	public:
		ParticleSystem(int size);

		void updateParticles(float dt);

	#pragma region Creators
			int createParticle();
			int createParticle(Vector3 pos, Quaternion rot);
	#pragma endregion
	
	#pragma region Setters
			void setParticleVel(int uid, Vector3 vel);
			void setParticleAngularVel(int uid, Vector3 vel);
	#pragma endregion
	
	#pragma region Getters
			inline Vector3 getPosition(int uid) { return pool->GetItem(uid).getPosition(); }
			inline Quaternion getRotation(int uid) { return pool->GetItem(uid).getRotation(); }
	#pragma endregion

	private:
		ParticlePool * pool;
};

#endif // !PARTICLE_SYSTEM_H