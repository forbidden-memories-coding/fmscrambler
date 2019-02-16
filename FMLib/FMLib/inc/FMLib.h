// FMLib.h : Includes all of the public API
#ifndef FMLIB_H
#define FMLIB_H

#pragma warning(disable:4251)

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
    class EXPORT FMLib
    {
    public:
        explicit    FMLib(std::string binPath);
                    FMLib(std::string slusPath, std::string mrgPath);
                    ~FMLib();

        Data        LoadData();
        bool        PatchImage();

        void        SetBin(std::string newPath);

    private:
        void        ExtractFiles();

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
    };
}

#endif // FMLIB_H