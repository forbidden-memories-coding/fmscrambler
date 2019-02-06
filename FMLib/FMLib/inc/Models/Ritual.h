// Ritual.h

#ifndef RITUAL_H
#define RITUAL_H

#include "Export.h"

namespace FMLib
{
    namespace Models
    {
        class EXPORT Ritual
        {
        public:
            int     RitualCard;
            int*    Cards;
            int     Result;

        };
    }
}
#endif // RITUAL_H