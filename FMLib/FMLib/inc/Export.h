// Export.h : Defines export symbols for cross-platform compatibility

#if defined _WIN32 || defined __CYGWIN__
    #ifdef LIBEXPORT
        #define EXPORT __declspec(dllexport)
    #else
        #define EXPORT __declspec(dllimport)
    #endif
#else
    #define EXPORT
#endif