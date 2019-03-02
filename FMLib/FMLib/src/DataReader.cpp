#include "DataReader.h"

#include <iostream>
#include <sstream>
#include <iomanip>
#include <limits>

namespace FMLib
{
    DataReader::DataReader()
    {

    }

    void DataReader::LoadDataFromSlus(std::fstream& slus, Data& dat)
    {

        std::cout << "\nSlus open? " << slus.is_open() << '\n';
        std::cout << "Slus position: " << slus.tellg() << '\n';

        // General card data
        slus.seekg(0x1C4A44, slus.beg);
        for (int i = 0; i < 722; ++i)
        {
            char card_info[4];
            slus.read(card_info, 4);

            int num_struct = (card_info[3] << 24) | (card_info[2] << 16) | (card_info[1] << 8) | card_info[0];
            dat.Cards[i].Id = i + 1;
            dat.Cards[i].Attack = (num_struct & 0x1FF) * 10;
            dat.Cards[i].Defense = (num_struct >> 9 & 0x1FF) * 10;
            dat.Cards[i].GuardianStar_Primary = num_struct >> 18 & 0xF;
            dat.Cards[i].GuardianStar_Secondary = num_struct >> 22 & 0xF;
            dat.Cards[i].Type = num_struct >> 26 & 0x1F;
        }

        // Cards level and attribute
        slus.seekg(0x1C5B33L, slus.beg);
        for (int i = 0; i < 722; ++i)
        {
            char str_num;
            slus.read(&str_num, 1);
            dat.Cards[i].Level = str_num & 0xF;
            dat.Cards[i].Attribute = str_num >> 4 & 0xF;
        }

        // Card name
        for(int i = 0; i < 722; ++i)
        {
            slus.seekg(0x1C6002 + i * 2, slus.beg);
            char str_num[2];
            slus.read(str_num, 2);
            unsigned short num = static_cast<unsigned short>(static_cast<unsigned int>(str_num[1] << 8) | str_num[0]);
            num &= std::numeric_limits<unsigned short>::max();
            slus.seekg(0x1C6800 + num - 0x6000, slus.beg);
            dat.Cards[i].Name = GetText(slus, dat.Dict);
        }
        
        // Card description
        for(int i = 0; i < 722; ++i)
        {
            slus.seekg(0x1B0A02 + i * 2, slus.beg);
            char str_num[2];
            slus.read(str_num, 2);
            unsigned short num = static_cast<unsigned short>(static_cast<unsigned int>(str_num[1] << 8) | str_num[0]);
            slus.seekg(0x1B11F4 + (num - 0x9F4));
            dat.Cards[i].Description = GetText(slus, dat.Dict);
        }

        // Duelist names
        for(int i = 0; i < 39; ++i)
        {
            slus.seekg(0x1C6652 + i * 2);
            char str_num[2];
            slus.read(str_num, 2);
            unsigned short num = static_cast<unsigned short>(static_cast<unsigned int>(str_num[1] << 8) | str_num[0]);
            slus.seekg(0x1C6800 + num - 0x6000);
            dat.Duelists[i] = Duelist(GetText(slus, dat.Dict));
        }
        
    }

    void DataReader::LoadDataFromWaMrg(std::fstream& mrg, Data& dat)
    {
        // Fusions
        mrg.seekg(0xB87800, mrg.beg);
        char fuseDat[0x10000];
        mrg.read(fuseDat, 0x10000);

        for(int i = 0; i < 722; ++i)
        {
            long position = 2 + i * 2;
            char posDat[2];
            posDat[0] = fuseDat[position];
            posDat[1] = fuseDat[position + 1];
            unsigned short num = static_cast<unsigned short>(static_cast<unsigned int>(posDat[1] << 8) | posDat[0]);
            position = num & std::numeric_limits<unsigned short>::max();

            if (position != 0)
            {
                int num1 = fuseDat[position++];
                if (num1 == 0)
                {
                    num1 = 511 - fuseDat[position++];
                }

                int num2 = num1;

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
        mrg.seekg(0xB85000, mrg.beg);
        char equipDat[10240];
        mrg.read(equipDat, 10240);
        
        while (true)
        {
            int position = 0;
            char num[2];
            num[0] = equipDat[position++];
            num[1] = equipDat[position++];
            int num6 = static_cast<unsigned short>(static_cast<unsigned int>(num[1] << 8) | num[0]);

            if (num6 == 0)
                break;

            num[0] = equipDat[position++];
            num[1] = equipDat[position++];
            int num7 = static_cast<unsigned short>(static_cast<unsigned int>(num[1] << 8) | num[0]);

            for(int num8 = 0; num8 < num7; ++num8)
            {
                num[0] = equipDat[position++];
                num[1] = equipDat[position++];
                int num9 = static_cast<unsigned short>(static_cast<unsigned int>(num[1] << 8) | num[0]);
                dat.Cards[num6 - 1].Equips.push_back(num9 - 1);
            }
        }

        // Card costs/passwords
        char starCost[722 * 8];
        mrg.seekg(0xFB9808, mrg.beg);
        mrg.read(starCost, 722 * 8);

        for(int i = 0; i < 722; ++i)
        {
            int position = 0;
            char cost[4];
            cost[0] = starCost[position++];
            cost[1] = starCost[position++];
            cost[2] = starCost[position++];
            cost[3] = starCost[position++];
            dat.Cards[i].Starchip.Cost = (cost[3] << 24) | (cost[2] << 16) | (cost[1] << 8) | cost[0];
            char pass[4];
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
            dat.Cards[i].Starchip.Password = (pass[3] << 24) | (pass[2] << 16) | (pass[1] << 8) | pass[0];
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

        std::cout << "\nStream position is " << std::hex << f.tellg() << '\n';
        std::cout << "File open? " << f.is_open() <<'\n';

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