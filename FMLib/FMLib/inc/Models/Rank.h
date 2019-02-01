// Rank.h

#ifndef RANK_H
#define RANK_H

#include "Export.h"

enum DropType
{
    SAPOW,
    SATEC,
    BCDPOW
};

class EXPORT Rank
{
public:
    int SaPow[722];
    int SaTec[722];
    int BcdPow[722];

    int* GetDropType(DropType type);
    
};

#endif // RANK_H