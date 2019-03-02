// FMLib.cpp

#include "FMLib.h"

namespace FMLib
{

    FMLib::FMLib(std::string binPath)
      : m_patcher(binPath),
        m_reader(),
        m_bin(binPath, std::ios::binary | std::ios::in | std::ios::out)
    {
        ExtractFiles();
        m_patcher.SetMrg(binPath.c_str());
        m_patcher.SetSlus(binPath.c_str());
    }

    FMLib::FMLib(std::string slusPath, std::string mrgPath)
      : m_patcher("", slusPath, mrgPath),
        m_reader(),
        m_slus(slusPath, std::ios::in | std::ios::out | std::ios::binary),
        m_mrg(mrgPath, std::ios::in | std::ios::out | std::ios::binary)
    {

    }

    Data* FMLib::LoadData()
    {
        Data* dat = new Data();
        m_reader.LoadAllData(m_slus, m_mrg, *dat);
        return dat;
    }

    bool FMLib::PatchImage()
    {
        if (m_bin.is_open()) return m_patcher.PatchImage();
        return false;
    }

    void FMLib::SetBin(const char* newPath)
    {
        std::string nP(newPath);
        if (m_bin.is_open()) m_bin.close();
        m_bin.open(nP, std::ios::app | std::ios::binary);
        if (!m_bin.is_open()) throw std::exception("Given file was not found or corrupt!");
        m_patcher.SetBin(nP.c_str());
    }

    FMLib::~FMLib()
    {

    }

    void FMLib::ExtractFiles()
    {
        m_slus.open("SLUS_014.11", std::ios::out | std::ios::binary);
        m_mrg.open("WA_MRG.MRG", std::ios::out | std::ios::binary);

        // Move to SLUS
        m_bin.seekg(CHUNK_SIZE * 24, m_bin.beg);

        // Extract SLUS
        constexpr unsigned int slusChunkAmt = 0x1D0800 / DATA_SIZE;
        Chunk slusChunks[slusChunkAmt];
        for(int i = 0; i < slusChunkAmt; ++i)
        {
            m_bin.read(slusChunks[i].syncPattern, 12);
            m_bin.read(slusChunks[i].address, 3);
            m_bin.read(&slusChunks[i].mode, 1);
            m_bin.read(slusChunks[i].subheader, 8);
            m_bin.read(slusChunks[i].data, DATA_SIZE);
            m_bin.read(slusChunks[i].errorDetect, 4);
            m_bin.read(slusChunks[i].errorCorrection, 276);
        }
        for(int i = 0; i < slusChunkAmt; ++i)
        {
            m_slus.write(slusChunks[i].data, DATA_SIZE);
        }
        
        // Move to MRG
        m_bin.seekg(CHUNK_SIZE * 10102, m_bin.beg);

        // Extract MRG
        constexpr unsigned int mrgChunkAmt = 0x2400000 / DATA_SIZE;
        Chunk mrgChunks[mrgChunkAmt];
        for(int i = 0; i < mrgChunkAmt; ++i)
        {
            m_bin.read(mrgChunks[i].syncPattern, 12);
            m_bin.read(mrgChunks[i].address, 3);
            m_bin.read(&mrgChunks[i].mode, 1);
            m_bin.read(mrgChunks[i].subheader, 8);
            m_bin.read(mrgChunks[i].data, DATA_SIZE);
            m_bin.read(mrgChunks[i].errorDetect, 4);
            m_bin.read(mrgChunks[i].errorCorrection, 276);
        }
        for(int i = 0; i < mrgChunkAmt; ++i)
        {
            m_mrg.write(mrgChunks[i].data, DATA_SIZE);
        }
        
        // Set flags
        m_slus.unsetf(std::ios::out);
        m_slus.setf(std::ios::app);

        m_mrg.unsetf(std::ios::out);
        m_slus.setf(std::ios::app);
    }

    extern "C" EXPORT IFMLib* __cdecl GetLibBin(const char* binPath)
    {
        return new FMLib(std::string(binPath));
    }

    extern "C" EXPORT IFMLib* __cdecl GetLibMrgSlus(const char* slusPath, const char* mrgPath)
    {
        return new FMLib(std::string(slusPath), std::string(mrgPath));
    }
}