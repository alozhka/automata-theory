cmake_minimum_required(VERSION 3.20)
project(lw1)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Подключение библиотеки из src/ExampleLib
# add_library(ExampleLib
#     src/ExampleLib/FileUtil.cpp
#     src/ExampleLib/FileUtil.h
# )

# target_include_directories(ExampleLib PUBLIC src/ExampleLib)

# Включаем поддержку тестов
enable_testing()

# include(FetchContent)
# FetchContent_Declare(
#         googletest
#         GIT_REPOSITORY https://github.com/google/googletest.git
#         GIT_TAG v1.14.0
# )
# For Windows: Prevent overriding the parent project's compiler/linker settings
# set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
# FetchContent_MakeAvailable(googletest)

# Создаем исполняемый файл для тестов
add_executable(main
    main.cpp
)

# Подключаем библиотеки к тестам
# target_link_libraries(main
#     gtest_main
# )

# Регистрируем тесты для запуска через ctest
# include(GoogleTest)
# gtest_discover_tests(main)