#!/bin/bash

apt-get update -y && apt-get install -y llvm-8 lldb-8
export LLDB_PATH=/usr/bin/lldb-8

cd /app/sdk
 git checkout v3.0.103
 ./build.sh
 cd .dotnet/
chmod +x dotnet-install.sh
./dotnet-install.sh

cd /app/diagnostics
git checkout 3.1.57502
./build.sh

dotnet build ./diagnostics.sln
./build.sh --test

cd /app/diagnostics/src/SOS/SOS.UnitTests
dotnet test

echo "remember to: export LLDB_PATH=/usr/bin/lldb-8"
