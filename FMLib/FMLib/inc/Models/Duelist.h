// Duelist.h
#ifndef DUELIST_H
#define DUELIST_H

#include "../Export.h"
#include "Rank.h"
#include <string>

namespace FMLib
{
    namespace Models
    {
        class EXPORT Duelist
        {
        public:
            Duelist(std::string name = "");
            
            std::string Name;
            int         Deck[722];
            Rank        Drop;
        };
    }
}
#endif // DUELIST_H
