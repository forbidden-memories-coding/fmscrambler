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

#endif //EXPORTED_H