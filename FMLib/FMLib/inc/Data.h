// Data.h : For all the data game has

#ifndef DATA_H
#define DATA_H

#include "Models/Card.h"
#include "Models/Duelist.h"
#include "Models/Fusion.h"
#include "Models/GameFile.h"
#include "Models/Rank.h"
#include "Models/Ritual.h"
#include "Models/Starchips.h"

#include "Export.h"

#include <map>

using namespace FMLib::Models;

typedef unsigned short BYTE;

namespace FMLib
{
    struct Data
    {
        Data();
        Card                    Cards[722];
        Duelist                 Duelists[39];
        std::string             BinPath;
        std::string             SlusPath;
        std::string             MrgPath;
        std::map<BYTE, char>    Dict;
        std::map<char, BYTE>    RDict;
    };
}

#endif //DATA_H