#include "Quaternion.h"
#include <math.h>

Quaternion::Quaternion(float w, float x, float y, float z)
{
	this->w = w;
	this->x = x;
	this->y = y;
	this->z = z;

	sqLength = w * w + x * x + y * y + z * z;
	length = sqrt(sqLength);
}

Quaternion::Quaternion(Vector3 vec, float w)
{
	this->w = w;
	this->x = vec.x;
	this->y = vec.y;
	this->z = vec.z;

	sqLength = w * w + x * x + y * y + z * z;
	length = sqrt(sqLength);
}

Quaternion Quaternion::operator*(Vector3 const& vec)
{
	float w, x, y, z;
	Vector3 vector(0, 0, 0);
	Vector3 quatVector(this->x, this->y, this->z);
	vector = vector * this->w + Vector3::Cross(quatVector, vec);

	return Quaternion(-Vector3::Dot(quatVector, vec), vector.x, vector.y, vector.z);
}

Quaternion Quaternion::operator*(float scalar)
{
	return Quaternion(this->w * scalar, this->x * scalar, this->y * scalar, this->z * scalar);
}

Quaternion Quaternion::operator*(Quaternion const& other)
{
	Vector3 vector(0, 0, 0);
	Vector3 vectorA(this->x, this->y, this->z), vectorB(other.x, other.y, other.z);

	vector =  vectorB * this->w + vectorA * other.w + Vector3::Cross(vectorA, vectorB);
	float w = this->w * other.w - Vector3::Dot(vectorA, vectorB);
	return Quaternion(w, vector.x, vector.y, vector.z);
}

Quaternion Quaternion::operator+(Quaternion const& other)
{
	return Quaternion(this->w + other.w, this->x + other.x, this->w + other.y, this->z + other.z);
}

Quaternion Quaternion::operator-()
{
	return Quaternion(-this->w, -this->x, -this->y, -this->z);
}

void Quaternion::normalize()
{
	float invLength = 1 / length;
	w = w * invLength;
	x = x * invLength;
	y = y * invLength;
	z = z * invLength;
}
