#ifndef PARTICLE_H
#define PARTICLE_H

#include "Vector3.h"
#include "Quaternion.h"

enum class UpdateMode
{
	INVALID = -1,
	KINEMATIC,
	EULER
};

class Particle
{
	public:
		int UID;
		Particle();
		Particle(Vector3 pos, Quaternion rot);

		Particle* getNext() const { return nextParticle; }
		void setNext(Particle* next) { nextParticle = next; }

	#pragma region Getters
		inline Vector3 getPosition() { return position; }
		inline Vector3 getVelocity() { return velocity; }
		inline Vector3 getAcceleration() { return acceleration; }

		inline Quaternion getRotation() { return rotation; }
		inline Vector3 getAngularVel() { return angularVelocity; }
		inline Vector3 getAngularAcc() { return angularAcceleration; }
	#pragma endregion

	#pragma region Setters
		inline void setPosition(Vector3 vec) { position = vec; }
		inline void setVelocity(Vector3 vec) { velocity = vec; }
		inline void setAcceleration(Vector3 vec) { acceleration = vec; }
		inline void setRotation(Quaternion quat) { rotation = quat; }
		inline void setAngularVel(Vector3 vec) { angularVelocity = vec; }
		inline void setAngularAcc(Vector3 vec) { angularAcceleration = vec; }
	#pragma endregion

	#pragma region Changers
		inline void changePosition(Vector3 vec) { position += vec; }
		inline void changeVelocity(Vector3 vec) { velocity += vec; }
		inline void changeAcceleration(Vector3 vec) { acceleration += vec; }
		inline void changeRotation(Quaternion quat) { rotation += quat; }
		inline void changeAngularVel(Vector3 vec) { angularVelocity += vec; }
		inline void changeAngularAcc(Vector3 vec) { angularAcceleration += vec; }
	#pragma endregion

		void update(float dt);

	private:
		Particle* nextParticle;

		float mass;
		Vector3 position;
		Vector3 velocity;
		Vector3 acceleration;

		Quaternion rotation;
		Vector3 angularVelocity;
		Vector3 angularAcceleration;

		UpdateMode updateMode = UpdateMode::KINEMATIC;
};

#endif // !PARTICLE_H