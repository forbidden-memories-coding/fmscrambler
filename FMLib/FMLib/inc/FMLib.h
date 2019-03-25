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
#include "Data.h"

namespace FMLib
{
    struct IFMLib
    {
        virtual Data* LoadData() = 0;
        virtual bool PatchImage(const char* imgName) = 0;
        virtual void SaveChanges() = 0;
        virtual void WriteData(const Data* dat) = 0;
        virtual void SetBin(const char* newPath) = 0;
        virtual const char* GetBinPath() = 0;
        virtual const char* GetSlusPath() = 0;
        virtual const char* GetMrgPath() = 0;
    };
    class FMLib : public IFMLib
    {
    public:
        explicit  FMLib(std::string binPath);
        FMLib(std::string slusPath, std::string mrgPath);
        ~FMLib();

        Data*   LoadData();
        bool    PatchImage(const char* imgName);
        void    WriteData(const Data* dat);
        void    SaveChanges();

        void  SetBin(const char* newPath);

        const char* GetBinPath();
        const char* GetSlusPath();
        const char* GetMrgPath();

        

    private:
        void  hex2bin(const char* src, char* target);
        void  ExtractFiles();

    private:
        struct Chunk
        {
            char syncPattern[12];
            char address[3];
            char mode;
            char subheader[8];
            char data[DATA_SIZE];
            char errorDetect[4];
            char errorCorrection[276];
        };

    private:
        DiscPatcher     m_patcher;
        DataReader      m_reader;
        std::fstream    m_bin;
        std::fstream    m_slus;
        std::fstream    m_mrg;
        std::string     m_binPath;
        std::string     m_slusPath;
        std::string     m_mrgPath;
    };

    extern "C" EXPORT IFMLib* CALL_CONV GetLibBin(const char* binPath);
    extern "C" EXPORT IFMLib* CALL_CONV GetLibMrgSlus(const char* slusPath, const char* mrgPath);
}

#endif // FMLIB_H