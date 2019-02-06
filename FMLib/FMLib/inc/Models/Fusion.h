// Fusions.h
#ifndef FUSION_H
#define FUSION_H

#include "Export.h"

namespace FMLib
{
    namespace Models
    {
        class EXPORT Fusion
        {
        public:
            Fusion(int Card1, int Card2, int Result);
            
            int Card1;
            int Card2;
            int Result;
        };
    }
}

#endif // FUSION_H