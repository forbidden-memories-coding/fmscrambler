#include "DiscPatcher.h"

namespace FMLib
{

    DiscPatcher::DiscPatcher(std::string bin, std::string slus, std::string mrg)
        : m_binFile(bin, std::ios::in | std::ios::out | std::ios::binary),
        m_edcTable{__EDCTABLE__}
    {
        if (slus != "")
        {
            m_slusFile.open(slus, std::ios::in | std::ios::out | std::ios::binary);
        }
        if (mrg != "")
        {
            m_mrgFile.open(mrg, std::ios::in | std::ios::out | std::ios::binary);
        }
    }

    DiscPatcher::~DiscPatcher()
    {
        if (m_binFile.is_open())
        {
            m_binFile.close();
        }
        if (m_slusFile.is_open())
        {
            m_slusFile.close();
        }
        if (m_mrgFile.is_open())
        {
            m_mrgFile.close();
        }
    }

    bool DiscPatcher::PatchImage(const char* imgName)
    {
        if (!m_binFile.is_open() || !m_slusFile.is_open() || !m_mrgFile.is_open()) return false;

        std::fstream newBin(std::string(imgName)+".bin", std::ios::out | std::ios::binary);

        m_binFile.seekg(0, m_binFile.beg);

        newBin << m_binFile.rdbuf();

        m_binFile.seekg(0, m_binFile.beg);
        
        m_slusFile.seekg(0, m_slusFile.end);
        int slusLength = static_cast<int>(m_slusFile.tellg());
        m_slusFile.seekg(0, m_slusFile.beg);

        m_mrgFile.seekg(0, m_mrgFile.end);
        int mrgLength = static_cast<int>(m_mrgFile.tellg());
        m_mrgFile.seekg(0, m_mrgFile.beg);

        int slusChunks = slusLength / DATA_SIZE;
        int mrgChunks = mrgLength / DATA_SIZE;

        newBin.seekg(24 * CHUNK_SIZE, newBin.beg);
        writeWithCrc(m_slusFile, newBin, slusChunks);
        newBin.seekg(10102 * CHUNK_SIZE, newBin.beg);
        writeWithCrc(m_mrgFile, newBin, mrgChunks);

        newBin.close();

        std::fstream newCue(std::string(imgName)+".cue", std::ios::out);
        const std::string fName = "FILE \""+std::string(imgName)+".bin\" BINARY\n";
        const char* cueSheet[] = {fName.c_str(), "  TRACK 01 MODE2/2352\n", "    INDEX 01 00:00:00\n"};
        const int lineSize[] = {fName.size(), sizeof("  TRACK 01 MODE2/2352\n"), sizeof("    INDEX 01 00:00:00\n")};
        for(int i = 0; i < 3; ++i)
            newCue.write(cueSheet[i], lineSize[i]-1);

        newCue.close();

        return true;
    }

    void DiscPatcher::SetBin(const char* newPath)
    {
        std::string nP(newPath);
        if (m_binFile.is_open()) m_binFile.close();
        m_binFile.open(nP, std::ios::in | std::ios::out | std::ios::binary);
        if (!m_binFile.is_open()) throw std::string("Given file was not found or corrupt!");
    }

    void DiscPatcher::SetSlus(const char* newPath)
    {
        std::string nP(newPath);
        if (m_slusFile.is_open()) m_slusFile.close();
        m_slusFile.open(nP, std::ios::in | std::ios::out | std::ios::binary);
        if (!m_slusFile.is_open()) throw std::string("Given file was not found or corrupt!");
    }

    void DiscPatcher::SetMrg(const char* newPath)
    {
        std::string nP(newPath);
        if (m_mrgFile.is_open()) m_mrgFile.close();
        m_mrgFile.open(nP, std::ios::in | std::ios::out | std::ios::binary);
        if (!m_mrgFile.is_open()) throw std::string("Given file was not found or corrupt!");
    }

    void DiscPatcher::writeWithCrc(std::fstream& f, std::fstream& bin, int chunks)
    {
        char crcCalc[2056];
        hex2bin("0000080000000800", crcCalc);
        char* postHeader = crcCalc + 8;

        for(int i = 0; i < chunks; ++i)
        {
            char* pHead = postHeader;
            unsigned int crc = 0;
            int len = 2056;
            f.read(pHead, DATA_SIZE);
            pHead = crcCalc;

            while(len--)
                crc = m_edcTable[(crc ^ *pHead++) & 0xFF] ^ (crc >> 8);

            char crcCh[4];
            crcCh[0] = static_cast<char>(crc >> 0);
            crcCh[1] = static_cast<char>(crc >> 8);
            crcCh[2] = static_cast<char>(crc >> 16);
            crcCh[3] = static_cast<char>(crc >> 24);
            bin.seekg(12, bin.cur);
            bin.seekg(3, bin.cur);
            bin.seekg(1, bin.cur);
            bin.seekg(8, bin.cur);
            bin.write(postHeader, DATA_SIZE);
            bin.write(crcCh, 4);
            bin.seekg(276, bin.cur);
        }
    }

    // Thanks to https://stackoverflow.com/a/40210047
    void DiscPatcher::hex2bin(const char* src, char* target)
    {
        auto char2int = [](char input)
        {
            if(input >= '0' && input <= '9')
                return input - '0';
            if(input >= 'A' && input <= 'F')
                return input - 'A' + 10;
            if(input >= 'a' && input <= 'f')
                return input - 'a' + 10;
            throw std::invalid_argument("Invalid input string");
        };

        while(*src && src[1])
        {
            *(target++) = char2int(*src)*16 + char2int(src[1]);
            src += 2;
        }
    }

    extern "C" EXPORT IDiscPatcher* CALL_CONV GetPatcher(const char* bin, const char* slus, const char* mrg)
    {
        return new DiscPatcher(std::string(bin), std::string(slus), std::string(mrg));
    }
}