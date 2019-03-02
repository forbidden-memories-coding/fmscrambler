// DataReader.h

#ifndef DATAREADER_H
#define DATAREADER_H

#include "Export.h"
#include "Data.h"

#include <fstream>

namespace FMLib
{
    class DataReader
    {
    public:
        EXPORT      DataReader();

        EXPORT void LoadDataFromSlus(std::fstream& slus, Data& dat);
        EXPORT void LoadDataFromWaMrg(std::fstream& mrg, Data& dat);
        EXPORT void LoadAllData(std::fstream& slus, std::fstream& mrg, Data& dat);

    private:
        std::string  GetText(std::fstream& f, std::map<BYTE, char> dic);
    };
}

#endif // DATAREADER_H