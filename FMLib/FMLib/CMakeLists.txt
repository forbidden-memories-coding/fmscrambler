add_library(
    FMLib
        "src/FMLib.cpp" 
        "inc/FMLib.h"
)

target_include_directories(
    FMLib 
        PUBLIC "inc"
)
