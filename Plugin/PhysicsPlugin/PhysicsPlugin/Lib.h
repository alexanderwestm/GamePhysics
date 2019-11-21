#ifndef LIB_H
#define LIB_H


#ifdef PLUGIN_EXPORT
#define PLUGIN_SYMBOL __declspec(dllexport)
#elif PLUGIN_IMPORT
#define PLUGIN_SYMBOL __declspec(dllimport)
#else
#define PLUGIN_SYMBOL
#endif

#endif // !LIB_H
