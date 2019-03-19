// Export.h : Defines export symbols for cross-platform compatibility
#ifndef EXPORTED_H
#define EXPORTED_H

#pragma warning(disable:4068)
#pragma GCC diagnostic ignored "-Wunknown-pragmas"
#pragma warning(disable:4251)

#if defined _WIN32 || defined __CYGWIN__
    #ifdef LIBEXPORT
        #define EXPORT __declspec(dllexport)
    #else
        #define EXPORT __declspec(dllimport)
    #endif
#else
    #define EXPORT
#endif

#if defined _WIN32 || defined __CYGWIN__
    #define CALL_CONV __cdecl
#else
    #define CALL_CONV
#endif

#endif //EXPORTED_H