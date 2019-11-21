#include "Plugin.h"
#include "Foo.h"

Foo* instance = 0;

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