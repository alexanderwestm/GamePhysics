#ifndef QUATERNION_H
#define QUATERNION_H

#include "Vector3.h"


class Quaternion
{
	public:
		static Quaternion identity;

		Quaternion();
		Quaternion(float w, float x, float y, float z);
		Quaternion(Vector3 vec, float w);

		Quaternion operator*(Vector3 const& vec);
		Quaternion operator*(float scalar);
		Quaternion operator*(Quaternion const& other);

		Quaternion operator+(Quaternion const& other);
		Quaternion operator-(Quaternion const& other);
		Quaternion operator-();

		Quaternion operator+=(Quaternion const& other);
		Quaternion operator=(Quaternion const& other);

		void normalize();

		float w, x, y, z;
		float length, sqLength;
	private:
};
#endif // !QUATERNION_H
