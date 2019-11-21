#include "Vector3.h"

Vector3 Vector3::Cross(Vector3 a, Vector3 b)
{
	float x, y, z;
	x = a.y * a.z - a.z * b.y;
	y = a.z * b.x - a.x * b.z;
	z = a.x * b.y - a.y * b.x;
	return Vector3(x, y, z);
}

float Vector3::Dot(Vector3 a, Vector3 b)
{
	return a.x * b.x + a.y * b.y + a.z * b.z;
}

Vector3::Vector3(float x, float y, float z)
{
	this->x = x;
	this->y = y;
	this->z = z;
}

Vector3 Vector3::operator*(float num)
{
	return Vector3(this->x * num, this->y * num, this->z * num);
}

Vector3 Vector3::operator+(Vector3 const& other)
{
	return Vector3(this->x + other.x, this->y + other.y, this->z + other.z);
}

float Vector3::getLength()
{
	return 0.0f;
}
