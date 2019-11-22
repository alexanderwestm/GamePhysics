#include "Vector3.h"
#include <math.h>

Vector3 Vector3::zero = Vector3(0, 0, 0);

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

Vector3::Vector3()
{
	x = y = z = 0;
	length = 0;
	sqLength = 0;
}

Vector3::Vector3(float x, float y, float z)
{
	this->x = x;
	this->y = y;
	this->z = z;

	sqLength = x * x + y * y + z * z;
	length = sqrt(sqLength);
}

Vector3 Vector3::operator*(float num)
{
	return Vector3(this->x * num, this->y * num, this->z * num);
}

Vector3 Vector3::operator+(Vector3 const& other)
{
	return Vector3(this->x + other.x, this->y + other.y, this->z + other.z);
}

Vector3 Vector3::operator+=(Vector3 const& other)
{
	this->x += other.x;
	this->y += other.y;
	this->z += other.z;
	return *this;
}

Vector3 Vector3::operator=(Vector3 const& other)
{
	this->x = other.x;
	this->y = other.y;
	this->z = other.z;
	return *this;
}
