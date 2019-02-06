#include "Models/Rank.h"

namespace FMLib
{
    namespace Models
    {
        int* Rank::GetDropType(DropType type)
        {
            switch (type)
            {
                case DropType::SAPOW:
                    return SaPow;

                case DropType::SATEC:
                    return SaTec;

                case DropType::BCDPOW:
                    return BcdPow;

                default:
                    return nullptr;
            }
        }
    }
}