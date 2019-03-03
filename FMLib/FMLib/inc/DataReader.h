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
        
        template<typename T>
        T ReadType(std::fstream& f);

        template<typename T>
        void ReadType(std::fstream& f, T *data, size_t size);
    };

    template<typename T>
    T DataReader::ReadType(std::fstream& f)
    {
        T obj;
        f.read(reinterpret_cast<char*>(&obj), sizeof(obj));
        return obj;
    }

    template<typename T>
    void DataReader::ReadType(std::fstream& f, T *data, size_t size)
    {
        f.read(reinterpret_cast<char*>(data), size);
    }
}

#endif // DATAREADER_H