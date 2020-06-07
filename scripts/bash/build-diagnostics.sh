#!/bin/bash


export LLDB_PATH=/usr/bin/lldb-8

cd /app/sdk
git checkout release/3.0.1xx
./build.sh

cd /app/sdk/.dotnet
chmod +x dotnet-install.sh
./dotnet-install.sh
ln -sf /app/sdk/.dotnet/dotnet /usr/bin/dotnet

cd /app/diagnostics
git checkout 3.1.57502
./build.sh

dotnet build ./diagnostics.sln
apt-get update -y && apt-get install -y llvm-8 lldb-8

./build.sh --test

cd /app/diagnostics/src/SOS/SOS.UnitTests
dotnet test

echo "remember to: export LLDB_PATH=/usr/bin/lldb-8"
