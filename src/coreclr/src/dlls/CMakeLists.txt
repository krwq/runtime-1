if(CLR_CMAKE_TARGET_WIN32)
    add_subdirectory(clretwrc)
endif(CLR_CMAKE_TARGET_WIN32)
add_subdirectory(dbgshim)
if (NOT (CLR_CMAKE_TARGET_WIN32 AND (CLR_CMAKE_TARGET_ARCH_I386 OR CLR_CMAKE_TARGET_ARCH_ARM) AND CLR_CMAKE_HOST_ARCH_AMD64))
    add_subdirectory(mscordbi)
    add_subdirectory(mscordac)
endif()
add_subdirectory(mscoree)
add_subdirectory(mscorpe)
add_subdirectory(mscorrc)
