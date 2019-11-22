#include "Plugin.h"
#include "Foo.h"

#include "ParticleSystem.h"

Foo* instance = 0;

ParticleSystem* system = nullptr;

int InitFoo(int f_new)
{
	if (!instance)
	{
		instance = new Foo(f_new);
		return 1;
	}
	return 0;
}

int DoFoo(int bar)
{
	if (instance)
	{
		return instance->foo(bar);
	}
	return 0;
}

int TermFoo()
{
	if (instance)
	{
		delete instance;
		instance = 0;
		return 1;
	}
	return 0;
}

int InitSystem()
{
	system = new ParticleSystem(50);
	return 1;
}

int TermSystem()
{
	if (system != nullptr)
	{
		delete system;
		system = nullptr;
		return 0;
	}
	return -1;
}

int CreateParticle()
{
	if (system != nullptr)
	{
		return system->createParticle();
	}
	return -1;
}

int CreateParticle(float posX, float posY, float posZ, float rotW, float rotX, float rotY, float rotZ)
{
	if (system != nullptr)
	{
		return system->createParticle(Vector3(posX, posY, posZ), Quaternion(rotW, rotX, rotY, rotZ));
	}
	return -1;
}

float* GetPos(int uid)
{
	if (system != nullptr)
	{
		float* arr = new float[3];
		Vector3 vec = system->getPosition(uid);
		arr[0] = vec.x;
		arr[1] = vec.y;
		arr[2] = vec.z;
		return arr;
	}
	return 0;
}

float* GetRot(int uid)
{
	if (system != nullptr)
	{
		float* arr = new float[4];
		Quaternion vec = system->getRotation(uid);
		arr[0] = vec.w;
		arr[1] = vec.x;
		arr[2] = vec.y;
		arr[3] = vec.z;
		return arr;
	}
	return 0;
}

int SetVel(int uid, float* arr)
{
	if (system != nullptr)
	{
		system->setParticleVel(uid, Vector3(arr[0], arr[1], arr[2]));
		return 0;
	}
	return -1;
}

int SetRotVel(int uid, float* arr) 
{
	if (system != nullptr)
	{
		system->setParticleAngularVel(uid, Vector3(arr[0], arr[1], arr[2]));
		return 0;
	}
	return -1;
}