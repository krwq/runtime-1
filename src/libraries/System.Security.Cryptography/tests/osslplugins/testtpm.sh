#!/bin/bash

ossplugings_path="$(dirname "$(realpath "${BASH_SOURCE[0]}")")"

ssc_tests_path="$(dirname "$ossplugings_path")"

ssc_path="$(dirname "$ssc_tests_path")"
ssc_src_path="$ssc_path/src"

libsrc_path="$(dirname "$ssc_path")"

src_path="$(dirname "$libsrc_path")"

nativelibs_path="$src_path/native/libs"


set -e
if [ "$1" != "run" ]; then
    (
        set -e
        cd "$nativelibs_path"
        dotnet build ./build-native.proj
    )

    (
        set -e
        cd "$ssc_src_path"
        dotnet build
    )
else
    # (
    #     set -e
    #     cd "$ssc_tests_path"
    #     dotnet test --filter "FullyQualifiedName=System.Security.Cryptography.Tests.OpenSslNamedKeysTests.Provider_OpenExistingPrivateKey"
    # )
    (
        set -e
        cd "$ssc_tests_path"
        dotnet test --filter "FullyQualifiedName=System.Security.Cryptography.Tests.OpenSslNamedKeysTests.Engine_OpenExistingTPMPrivateKey"
    )
fi
