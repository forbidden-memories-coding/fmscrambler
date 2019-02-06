// FMLib.h : Includes all of the public API
#ifndef FMLIB_H
#define FMLIB_H

#include "Models/Card.h"
#include "Models/Duelist.h"
#include "Models/Fusion.h"
#include "Models/GameFile.h"
#include "Models/Rank.h"
#include "Models/Ritual.h"
#include "Models/Starchips.h"

#include "DataReader.h"
#include "DiscPatcher.h"

namespace FMLib
{
    class FMLib
    {
    public:
        FMLib();
        virtual ~FMLib();
    };
}

#endif // FMLIB_H