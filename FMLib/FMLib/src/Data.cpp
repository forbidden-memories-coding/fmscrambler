#include "Data.h"

namespace FMLib
{
    // Init character table
    Data::Data()
    {
        Dict[0x18] = 'A';
        Dict[0x2D] = 'B';
        Dict[0x2B] = 'C';
        Dict[0x20] = 'D';
        Dict[0x25] = 'E';
        Dict[0x31] = 'F';
        Dict[0x29] = 'G';
        Dict[0x23] = 'H';
        Dict[0x1A] = 'I';
        Dict[0x3B] = 'J';
        Dict[0x33] = 'K';
        Dict[0x2A] = 'L';
        Dict[0x1E] = 'M';
        Dict[0x2C] = 'N';
        Dict[0x21] = 'O';
        Dict[0x2F] = 'P';
        Dict[0x3E] = 'Q';
        Dict[0x26] = 'R';
        Dict[0x1D] = 'S';
        Dict[0x1C] = 'T';
        Dict[0x35] = 'U';
        Dict[0x39] = 'V';
        Dict[0x22] = 'W';
        Dict[0x46] = 'X';
        Dict[0x24] = 'Y';
        Dict[0x3F] = 'Z';
        Dict[0x03] = 'a';
        Dict[0x15] = 'b';
        Dict[0x0F] = 'c';
        Dict[0x0C] = 'd';
        Dict[0x01] = 'e';
        Dict[0x13] = 'f';
        Dict[0x10] = 'g';
        Dict[0x09] = 'h';
        Dict[0x05] = 'i';
        Dict[0x34] = 'j';
        Dict[0x16] = 'k';
        Dict[0x0A] = 'l';
        Dict[0x0E] = 'm';
        Dict[0x06] = 'n';
        Dict[0x04] = 'o';
        Dict[0x14] = 'p';
        Dict[0x37] = 'q';
        Dict[0x08] = 'r';
        Dict[0x07] = 's';
        Dict[0x02] = 't';
        Dict[0x0D] = 'u';
        Dict[0x19] = 'v';
        Dict[0x12] = 'w';
        Dict[0x36] = 'x';
        Dict[0x11] = 'y';
        Dict[0x32] = 'z';
        Dict[0x38] = '0';
        Dict[0x3D] = '1';
        Dict[0x3A] = '2';
        Dict[0x41] = '3';
        Dict[0x4A] = '4';
        Dict[0x42] = '5';
        Dict[0x4E] = '6';
        Dict[0x45] = '7';
        Dict[0x57] = '8';
        Dict[0x59] = '9';
        Dict[0x00] = ' ';
        Dict[0x30] = '-';
        Dict[0x3C] = '#';
        Dict[0x43] = '&';
        Dict[0x0B] = '.';
        Dict[0x1F] = ',';
        Dict[0x55] = 'a';
        Dict[0x17] = '!';
        Dict[0x1B] = '\'';
        Dict[0x27] = '<';
        Dict[0x28] = '>';
        Dict[0x2E] = '?';
        Dict[0x44] = '/';
        Dict[0x48] = ':';
        Dict[0x4B] = ')';
        Dict[0x4C] = '(';
        Dict[0x4F] = '$';
        Dict[0x50] = '*';
        Dict[0x51] = '>';
        Dict[0x54] = '<';
        Dict[0x40] = '"';
        Dict[0x56] = '+';
        Dict[0x5B] = '%';
        for(auto pair : Dict)
        {
            RDict[pair.second] = pair.first;
        }
    }
}