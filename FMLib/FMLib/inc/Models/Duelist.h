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
        struct EXPORT Duelist
        {
            Duelist(std::string name = "");
            
            std::string Name;
            int         Deck[722];
            Rank        Drop;
        };
    }
}
#endif // DUELIST_H
