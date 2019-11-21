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


#ifdef __cplusplus__
}
#else

#endif

#endif // !PLUGIN_H