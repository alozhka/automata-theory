cmake_minimum_required(VERSION 3.20)
project(lw1)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(ExampleLib
    lw1/src/ExampleLib/FileUtil.cpp
    lw1/src/ExampleLib/FileUtil.h
)

target_include_directories(ExampleLib PUBLIC src/ExampleLib)

enable_testing()

include(FetchContent)
FetchContent_Declare(
        googletest
        GIT_REPOSITORY https://github.com/google/googletest.git
        GIT_TAG v1.14.0
)
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

add_executable(tests
    lw1/tests/test_main.cpp
)

target_link_libraries(tests
    ExampleLib
    gtest_main
)

include(GoogleTest)
gtest_discover_tests(tests)