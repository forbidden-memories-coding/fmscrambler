cmake_minimum_required(VERSION 3.10)

add_definitions(-DLIBEXPORT)

add_library(
    FMLib
        "src/FMLib.cpp"
        "src/Models/Duelist.cpp"
        "src/Models/Fusion.cpp"
        "src/Models/Rank.cpp"
        "inc/FMLib.h"
        "inc/Export.h"
        "inc/Models/Card.h"
        "inc/Models/Duelist.h"
        "inc/Models/Fusion.h"
        "inc/Models/GameFile.h"
        "inc/Models/Rank.h"
        "inc/Models/Ritual.h"
        "inc/Models/Starchips.h"
)

target_include_directories(
    FMLib 
        PUBLIC "inc"
        PUBLIC "Models"
)
