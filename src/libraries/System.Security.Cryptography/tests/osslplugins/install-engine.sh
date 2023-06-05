#!/bin/sh

set -e

# openssl command only checks if loads correctly

openssl engine -t -c `pwd`/dntest.so &&
    sudo cp dntest.so /usr/lib/x86_64-linux-gnu/engines-3/ &&
    echo 'Finished installation.'
