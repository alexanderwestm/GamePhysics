#ifndef VECTOR3_H
#define VECTOR3_H

class Vector3
{
	public:
		static Vector3 zero;

		static Vector3 Cross(Vector3 a, Vector3 b);
		static float Dot(Vector3 a, Vector3 b);

		Vector3();
		Vector3(float x, float y, float z);

		Vector3 operator*(float num);
		Vector3 operator+(Vector3 const& other);
		Vector3 operator+=(Vector3 const& other);
		Vector3 operator=(Vector3 const& other);

		float x, y, z;
		float length, sqLength;
	private:
};
#endif // !VECTOR3_H
