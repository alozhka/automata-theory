cmake_minimum_required(VERSION 3.20)
project(lw1)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

message(STATUS " Используемый C++ компилятор: ${CMAKE_CXX_COMPILER}")
message(STATUS " Версия компилятора: ${CMAKE_CXX_COMPILER_VERSION}")

add_library(ExampleLib
        src/ExampleLib/FileUtil.cpp
        src/ExampleLib/FileUtil.h
        src/ExampleLib/TextUtil.cpp
        src/ExampleLib/TextUtil.h
)

target_include_directories(ExampleLib PUBLIC src/ExampleLib)

enable_testing()

add_subdirectory(googletest-1.14.0)

add_executable(tests
        tests/test_main.cpp
)

target_link_libraries(tests
        ExampleLib
        gtest_main
)

include(GoogleTest)
gtest_discover_tests(tests)