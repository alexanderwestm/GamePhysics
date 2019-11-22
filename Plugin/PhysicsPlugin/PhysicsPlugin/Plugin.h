#ifndef PLUGIN_H
#define PLUGIN_H

#include "Lib.h"

#ifdef __cplusplus__
extern "C"
{
#else

#endif


PLUGIN_SYMBOL int InitFoo(int f_new);
PLUGIN_SYMBOL int DoFoo(int bar);
PLUGIN_SYMBOL int TermFoo();

PLUGIN_SYMBOL int InitSystem();
PLUGIN_SYMBOL int TermSystem();
PLUGIN_SYMBOL int CreateParticle();
PLUGIN_SYMBOL int CreateParticle(float posX, float posY, float posZ, float rotW, float rotX, float rotY, float rotZ);
PLUGIN_SYMBOL float* GetPos(int uid);
PLUGIN_SYMBOL float* GetRot(int uid);
PLUGIN_SYMBOL int SetVel(int uid, float* arr);
PLUGIN_SYMBOL int SetRotVel(int uid, float* arr);


#ifdef __cplusplus__
}
#else

#endif

#endif // !PLUGIN_H