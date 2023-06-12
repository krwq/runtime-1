#!/bin/sh

set -e

# openssl command only checks if loads correctly

# openssl engine -t -c `pwd`/dntest.so &&
#     sudo cp dntest.so /usr/lib/x86_64-linux-gnu/engines-3/ &&
#     echo 'Finished installation.'
# /usr/lib/x86_64-linux-gnu/ossl-modules/
sudo cp dntestprov.so /usr/lib/x86_64-linux-gnu/ossl-modules/ &&
    openssl list -providers -provider dntestprov # unfortunatelly even if there are errors this has error code 0
