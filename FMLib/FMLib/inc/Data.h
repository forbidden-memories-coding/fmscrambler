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
#include <bitset>

using namespace FMLib::Models;


typedef unsigned short BYTE;

namespace FMLib
{
    class EXPORT Data
    {
    public:
        Data() = default;
        ~Data() = default;
        
    public:
        Card                    Cards[722];
        Duelist                 Duelists[39];
        std::string             BinPath;
        std::string             SlusPath;
        std::string             MrgPath;
        std::map<BYTE, char>    Dict;
        std::map<BYTE, char>    RDict;
    };
}

#endif //DATA_H