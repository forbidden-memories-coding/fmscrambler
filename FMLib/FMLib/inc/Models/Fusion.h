// Fusions.h
#ifndef FUSION_H
#define FUSION_H

#include "Export.h"

namespace FMLib
{
    namespace Models
    {
        struct EXPORT Fusion
        {
            Fusion(int Card1 = 0, int Card2 = 0, int Result = 0);
            
            int Card1;
            int Card2;
            int Result;
        };
    }
}

#endif // FUSION_H