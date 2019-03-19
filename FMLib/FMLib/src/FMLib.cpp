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
        if (!m_bin.is_open()) throw std::string("Given file was not found or corrupt!");
        m_patcher.SetBin(nP.c_str());
    }

    void FMLib::hex2bin(const char* src, char* target)
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

    void FMLib::WriteData(const Data* dat)
    {

        // Field's fusions positions in WA_MRG?
        int numArray[] = {
            0xB87800,
            0xBFD000,
            0xC72800,
            0xCE8000,
            0xD5D800,
            0xDD3000,
            0xE48800
        };

        unsigned char memStream1[1444];
        unsigned char memStream2[64092];

        // Write fusions
        int pos1 = 2;
        int pos2 = 2;
        for(auto card : dat->Cards)
        {
            short num1 = card.Fusions.size() != 0 ? pos2 + 1444 : 0;
            memStream1[pos1++] = num1 & 0xFF; 
            memStream1[pos1++] = num1 >> 8 & 0xFF;
            if (card.Fusions.size() != 0)
            {
                if (card.Fusions.size() < 256)
                {
                    memStream2[pos2++] = card.Fusions.size();
                }
                else
                {
                    memStream2[pos2++] = 0;
                    memStream2[pos2++] = std::abs((int)card.Fusions.size() - 511);
                }
                for (int i = 0; i < card.Fusions.size(); ++i)
                {
                    int num2 = card.Fusions[i].Card2 + 1 & 0xFF;
                    int num3 = card.Fusions[i].Result + 1 & 0xFF;
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = card.Fusions[i].Card2 + 1 >> 8 & 3 | (card.Fusions[i].Result + 1 >> 8 & 3) << 2;
                    if (i < card.Fusions.size() - 1)
                    {
                        num4 = card.Fusions[i+1].Card2 + 1 & 0xFF;
                        num5 = card.Fusions[i+1].Result + 1 & 0xFF;
                        num6 |= (card.Fusions[i+1].Card2 + 1 >> 8 & 3) << 4 |
                                (card.Fusions[i+1].Result + 1 >> 8 & 3) << 6;
                        ++i;
                    }
                    memStream2[pos2++] = num6 & 0xFF;
                    memStream2[pos2++] = num2 & 0xFF;
                    memStream2[pos2++] = num3 & 0xFF;
                    if (num4 != 0 || num5 != 0)
                    {
                        memStream2[pos2++] = num4 & 0xFF;
                        memStream2[pos2++] = num5 & 0xFF;
                    }
                }
            }
        }
        while(pos2 < 64092)
        {
            memStream2[pos2++] = 0xEE;
        }

        for(int num : numArray)
        {
            m_mrg.seekg(num);
            m_mrg.write(reinterpret_cast<char*>(memStream1), sizeof(memStream1));
            m_mrg.write(reinterpret_cast<char*>(memStream2), sizeof(memStream2));
        }

        // Write ATK/DEF, Guardian Start, Types, Attributes
        m_slus.seekg(0x1C4A44);
        unsigned char memStream[2888];
        int pos = 0;
        for(int i = 0; i < 722; ++i)
        {
            int val = (dat->Cards[i].Attack / 10 & 0x1FF) | (dat->Cards[i].Defense / 10 & 0x1FF) << 9 |
                        (dat->Cards[i].GuardianStar_Secondary & 0xF) << 18 |
                        (dat->Cards[i].GuardianStar_Primary & 0xF) << 22 | (dat->Cards[i].Type & 0x1F) << 26;
            memStream[pos++] = val & 0xFF;
            memStream[pos++] = val >> 8 & 0xFF;
            memStream[pos++] = val >> 16 & 0xFF;
            memStream[pos++] = val >> 24 & 0xFF;
        }
        m_slus.write(reinterpret_cast<char*>(memStream), sizeof(memStream));

        // Write decks and card drops
        for(int i = 0; i < 39; ++i)
        {
            int num = 0xE9B000 + 0x1800 * i;

            m_mrg.seekg(num);
            unsigned char memStream[1444];
            int pos = 0;
            for(int t : dat->Duelists[i].Deck)
            {
                short val = t;
                memStream[pos++] = val & 0xFF;
                memStream[pos++] = val >> 8 & 0xFF;
            }
            m_mrg.write(reinterpret_cast<char*>(memStream), sizeof(memStream));

            m_mrg.seekg(num + 0x5B4);
            for(int t : dat->Duelists[i].Drop.SaPow)
            {
                short val = t;
                memStream[pos++] = val & 0xFF;
                memStream[pos++] = val >> 8 & 0xFF;
            }
            m_mrg.write(reinterpret_cast<char*>(memStream), sizeof(memStream));

            m_mrg.seekg(num + 0xB68);
            for(int t : dat->Duelists[i].Drop.BcdPow)
            {
                short val = t;
                memStream[pos++] = val & 0xFF;
                memStream[pos++] = val >> 8 & 0xFF;
            }
            m_mrg.write(reinterpret_cast<char*>(memStream), sizeof(memStream));

            m_mrg.seekg(num + 0x111C);
            for(int t : dat->Duelists[i].Drop.SaTec)
            {
                short val = t;
                memStream[pos++] = val & 0xFF;
                memStream[pos++] = val >> 8 & 0xFF;
            }
            m_mrg.write(reinterpret_cast<char*>(memStream), sizeof(memStream));
        }

        // Write starchips
        m_mrg.seekg(0xFB9808);
        for(int i = 0; i < 722; ++i)
        {
            unsigned char cost_arr[4];
            cost_arr[0] = dat->Cards[i].Starchip.Cost & 0xFF;
            cost_arr[1] = dat->Cards[i].Starchip.Cost >> 8 & 0xFF;
            cost_arr[2] = dat->Cards[i].Starchip.Cost >> 16 & 0xFF;
            cost_arr[3] = dat->Cards[i].Starchip.Cost >> 24 & 0xFF;
            unsigned char pass_arr[4];
            hex2bin(dat->Cards[i].Starchip.PasswordStr.c_str(), reinterpret_cast<char*>(pass_arr));
            int offset = 0;
            for(int j = sizeof(cost_arr) - 2; j >= 2; --j)
            {
                if (cost_arr[j] == 0) offset++;
                else break;
            }
            for(int j = 0; j < sizeof(cost_arr) - offset - 1; ++j)
            {
                m_mrg.write(reinterpret_cast<char*>(&cost_arr[j]), 1);
            }
            for(int j = 0; j < offset; ++j) m_mrg.write(0, 1);
            m_mrg.seekg(1, m_mrg.cur);
            for(int j = sizeof(pass_arr) - 1; j >= 0; --j)
            {
                m_mrg.write(reinterpret_cast<char*>(&pass_arr[j]), 1);
            }
        }
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
        m_slus.close();
        m_slus.open("SLUS_014.11", std::ios::out | std::ios::binary | std::ios::in);

        m_mrg.close();
        m_mrg.open("WA_MRG.MRG", std::ios::out | std::ios::binary | std::ios::in);
        /*m_slus.unsetf(std::ios::o);
        m_slus.setf(std::ios::app);

        m_mrg.unsetf(std::ios::out);
        m_slus.setf(std::ios::app);*/
    }

    extern "C" EXPORT IFMLib* CALL_CONV GetLibBin(const char* binPath)
    {
        return new FMLib(std::string(binPath));
    }

    extern "C" EXPORT IFMLib* CALL_CONV GetLibMrgSlus(const char* slusPath, const char* mrgPath)
    {
        return new FMLib(std::string(slusPath), std::string(mrgPath));
    }
}