// Card.h

#ifndef CARD_H
#define CARD_H

#include "../Export.h"
#include "Fusion.h"
#include "Ritual.h"
#include "Starchips.h"
#include <string>
#include <list>

namespace FMLib
{
    namespace Models
    {
        class EXPORT Card
        {
        public:
            int                 Id;
            std::string         Name;
            std::string         Description;
            int                 Attack;
            int                 Defense;
            int                 Attribute;
            int                 Level;
            int                 Type;
            int                 GuardianStar_Primary;
            int                 GuardianStar_Secondary;
            std::list<Fusion>   Fusions;
            std::list<int>      Equips;
            Ritual              Rituals;
            Starchips           Starchip;
        };
    }
}


#endif // CARD_H
