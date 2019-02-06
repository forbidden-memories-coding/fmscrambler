#ifndef DISCPATCHER_H
#define DISCPATCHER_H

#include "Export.h"

#include <string>
#include <fstream>

namespace FMLib
{
    class EXPORT DiscPatcher
    {
    public:
        explicit    DiscPatcher(std::string file);

        int         PatchImage();
        void        ListDirectories();
        std::string GetName(char* data, int size);

    private:
        std::fstream m_binFile;
    };
}

#endif // DISCPATCHER_H