// DataReader.h

#ifndef DATAREADER_H
#define DATAREADER_H

#include "Export.h"

namespace FMLib
{
    namespace Randomizer
    {
        class EXPORT DataReader
        {
        public:
            explicit    DataReader();

            void        LoadDataFromSlus();
            void        LoadDataFromWaMrg();

        };
    }
}

#endif // DATAREADER_H