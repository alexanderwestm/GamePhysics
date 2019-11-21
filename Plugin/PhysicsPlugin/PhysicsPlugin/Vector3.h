#ifndef VECTOR3_H
#define VECTOR3_H

class Vector3
{
	public:
		static Vector3 Cross(Vector3 a, Vector3 b);
		static float Dot(Vector3 a, Vector3 b);

		Vector3(float x, float y, float z);

		Vector3 operator*(float num);
		Vector3 operator+(Vector3 const& other);

		float getLength();

		float x, y, z;
	private:
};

#endif // !VECTOR3_H
