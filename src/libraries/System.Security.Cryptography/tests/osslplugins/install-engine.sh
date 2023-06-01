#!/bin/sh

set -e

# cp dntest.so /usr/lib/x86_64-linux-gnu/engines-3/
openssl engine -t -c `pwd`/dntest.so
