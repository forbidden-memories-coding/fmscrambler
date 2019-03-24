#include "DataReader.h"

#include <iostream>
#include <sstream>
#include <iomanip>
#include <climits>
#include <cstring>

namespace FMLib
{
    DataReader::DataReader()
    {

    }

    void DataReader::LoadDataFromSlus(std::fstream& slus, Data& dat)
    {

        // General card data
        slus.seekg(0x1C4A44);
        for (int i = 0; i < 722; ++i)
        {
            int num_struct = ReadType<int>(slus);

            dat.Cards[i].Id = i + 1;
            dat.Cards[i].Attack = (num_struct & 0x1FF) * 10;
            dat.Cards[i].Defense = (num_struct >> 9 & 0x1FF) * 10;
            dat.Cards[i].GuardianStar_Primary = num_struct >> 18 & 0xF;
            dat.Cards[i].GuardianStar_Secondary = num_struct >> 22 & 0xF;
            dat.Cards[i].Type = num_struct >> 26 & 0x1F;
        }

        // Cards level and attribute
        slus.seekg(0x1C5B33L);
        for (int i = 0; i < 722; ++i)
        {
            unsigned char str_num = ReadType<unsigned char>(slus);
            dat.Cards[i].Level = str_num & 0xF;
            dat.Cards[i].Attribute = str_num >> 4 & 0xF;
        }

        // Card name
        for(int i = 0; i < 722; ++i)
        {
            slus.seekg(0x1C6002 + i * 2);
            unsigned short num = ReadType<unsigned short>(slus);
            slus.seekg(0x1C6800 + num - 0x6000);
            dat.Cards[i].Name = GetText(slus, dat.Dict);
        }
        
        // Card description
        for(int i = 0; i < 722; ++i)
        {
            slus.seekg(0x1B0A02 + i * 2);
            unsigned short off = ReadType<unsigned short>(slus);
            slus.seekg(0x1B11F4 + (off - 0x9F4));
            dat.Cards[i].Description = GetText(slus, dat.Dict);
        }

        // Duelist names
        slus.seekg(0x1C93DB);
        for(int i = 0; i < 39; ++i)
        {
            slus.seekg(0x1C6652 + i * 2);
            slus.seekg(0x1C6800 + ReadType<unsigned short>(slus) - 0x6000);
            dat.Duelists[i] = Duelist(GetText(slus, dat.Dict));
        }
        
    }

    void DataReader::LoadDataFromWaMrg(std::fstream& mrg, Data& dat)
    {
        // Fusions
        mrg.seekg(0xB87800);
        unsigned char fuseDat[0x10000];
        //mrg.read(fuseDat, 0x10000);
        ReadType(mrg, fuseDat, sizeof(unsigned char) * 0x10000);

        for(int i = 0; i < 722; ++i)
        {
            long position = 2 + i * 2;
            unsigned char posDat[2];
            posDat[0] = fuseDat[position];
            posDat[1] = fuseDat[position + 1];
            unsigned short num = static_cast<unsigned short>(posDat[1] << 8 | posDat[0]);
            position = num & 0xFFFF;

            if (position != 0)
            {
                int fusionAmt = fuseDat[position++];
                if (fusionAmt == 0)
                {
                    fusionAmt = 511 - fuseDat[position++];
                }

                int num2 = fusionAmt;

                while (num2 > 0)
                {
                    int num3 = fuseDat[position++];
                    int num4 = fuseDat[position++];
                    int num5 = fuseDat[position++];
                    int num6 = fuseDat[position++];
                    int num7 = fuseDat[position++];
                    int num9 = (num3 & 3) << 8 | num4;
                    int num11 = (num3 >> 2 & 3) << 8 | num5;
                    int num13 = (num3 >> 4 & 3) << 8 | num6;
                    int num15 = (num3 >> 6 & 3) << 8 | num7;

                    dat.Cards[i].Fusions.push_back(Fusion(i + 1, num9 - 1, num11 - 1));
                    --num2;

                    if (num2 <= 0) continue;

                    dat.Cards[i].Fusions.push_back(Fusion(i + 1, num13 - 1, num15 - 1));
                    --num2;
                }
            }
        }

        // Equips
        mrg.seekg(0xB85000);
        unsigned char equipDat[0x2800];
        //mrg.read(equipDat, 0x2800);
        ReadType(mrg, equipDat, sizeof(unsigned char) * 0x2800);

        int position = 0;
        while (true)
        {
            unsigned char num[2];
            num[0] = equipDat[position++];
            num[1] = equipDat[position++];
            unsigned short equipId = static_cast<unsigned short>(num[1] << 8 | num[0]);

            if (equipId == 0)
                break;

            num[0] = equipDat[position++];
            num[1] = equipDat[position++];
            unsigned short monsterNum = static_cast<unsigned short>(num[1] << 8 | num[0]);

            for(int i = 0; i < monsterNum; ++i)
            {
                num[0] = equipDat[position++];
                num[1] = equipDat[position++];
                unsigned short monsterId = static_cast<unsigned short>(num[1] << 8 | num[0]);
                dat.Cards[equipId - 1].Equips.push_back(monsterId - 1);
            }
        }

        // Card costs/passwords
        unsigned char starCost[722 * 8];
        mrg.seekg(0xFB9808, mrg.beg);
        //mrg.read(starCost, 722 * 8);
        ReadType(mrg, starCost, sizeof(unsigned char) * 722 * 8);

        position = 0;
        for(int i = 0; i < 722; ++i)
        {
            unsigned char cost[4];
            cost[0] = starCost[position++];
            cost[1] = starCost[position++];
            cost[2] = starCost[position++];
            cost[3] = starCost[position++];
            dat.Cards[i].Starchip.Cost = (cost[3] << 24) | (cost[2] << 16) | (cost[1] << 8) | cost[0];
            unsigned char pass[4];
            pass[0] = starCost[position++];
            pass[1] = starCost[position++];
            pass[2] = starCost[position++];
            pass[3] = starCost[position++];

            std::string resPass;
            for(int j = 3; j >= 0; --j)
            {
                std::stringstream ss;
                ss << std::setfill('0') << std::setw(2) << std::hex << (int)pass[j];
                resPass += ss.str();
            }
            dat.Cards[i].Starchip.PasswordStr = resPass;
            dat.Cards[i].Starchip.Password = (strcmp(resPass.c_str(), "fffffffe") != 0) ? std::stoi(resPass) : 0;
                
        }
        

        // Read duelist decks and card drops
        for(int i = 0; i < 39; ++i)
        {
            int num = 0xE9B000 + 0x1800 * i;
            mrg.seekg(num);

            unsigned char memStream[1444];

            ReadType(mrg, memStream, 1444);
            position = 0;
            for(int j = 0; j < 722; ++j)
            {
                unsigned char num[2];
                num[0] = memStream[position++];
                num[1] = memStream[position++];
                dat.Duelists[i].Deck[j] = static_cast<int>(num[1] << 8 | num[0]);
            }

            mrg.seekg(num + 0x5B4);

            ReadType(mrg, memStream, 1444);
            position = 0;
            for(int j = 0; j < 722; ++j)
            {
                unsigned char num[2];
                num[0] = memStream[position++];
                num[1] = memStream[position++];
                dat.Duelists[i].Drop.SaPow[j] = static_cast<int>(num[1] << 8 | num[0]);
            }

            mrg.seekg(num + 0xB68);

            ReadType(mrg, memStream, 1444);
            position = 0;
            for(int j = 0; j < 722; ++j)
            {
                unsigned char num[2];
                num[0] = memStream[position++];
                num[1] = memStream[position++];
                dat.Duelists[i].Drop.BcdPow[j] = static_cast<int>(num[1] << 8 | num[0]);
            }

            mrg.seekg(num + 0x111C);

            ReadType(mrg, memStream, 1444);
            position = 0;
            for(int j = 0; j < 722; ++j)
            {
                unsigned char num[2];
                num[0] = memStream[position++];
                num[1] = memStream[position++];
                dat.Duelists[i].Drop.SaTec[j] = static_cast<int>(num[1] << 8 | num[0]);
            }
        }
    }

    void DataReader::LoadAllData(std::fstream& slus, std::fstream& mrg, Data& dat)
    {
        LoadDataFromSlus(slus, dat);
        LoadDataFromWaMrg(mrg, dat);
    }


    std::string DataReader::GetText(std::fstream& f, std::map<BYTE, char> dic)
    {
        std::string res = "";

        while (true)
        {
            char b;
            f.read(&b, 1);
            BYTE b_num = b;

            if (dic.find(b_num) != dic.end())
            {
                res += dic[b_num];
            }
            else if (b_num == 65534)
            {
                res += "\r\n";
            }
            else
            {
                if (b_num == 65535)
                    break;

                std::stringstream ss;
                ss << std::setfill('0') << std::setw(2) << std::hex << b_num;
                res += "[" + ss.str() + "]";
            }
            
        }

        return res;
    }
}
