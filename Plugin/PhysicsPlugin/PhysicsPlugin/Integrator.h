#ifndef INTEGRATOR_H
#define INTEGRATOR_H

#include "Vector3.h"
#include "Quaternion.h"

class Integrator
{
	public:
		static void UpdatePositionEulerExplicit(Vector3& pos, Vector3& vel, Vector3& acc, float dt);
		static void UpdateRotationEulerExplicit(Quaternion& rot, Vector3& vel, Vector3& acc, float dt);

		static void UpdatePositionKinematic(Vector3& pos, Vector3& vel, Vector3& acc, float dt);
		static void UpdateRotationKinematic(Quaternion& rot, Vector3& vel, Vector3& acc, float dt);
};

#endif // !INTEGRATOR_H
