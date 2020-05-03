Load up the llvm container

make llvm

Chdir to the test_debug path:

cd ./source/own/test_debug

Build the application.

dotnet build

Run the application

dotnet run

Open another terminal

Load up the llvm container

docker ps
docker exec -it <container_id> bash

Chdir to the test_debug path:

cd ./source/own/test_debug

Install dotnet-dump. (more info here: https://docs.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-dump)

dotnet tool install -g dotnet-dump

find the process with the name test_debug:

ps aux|grep test_debug

launch dotnet-dump with the processid of the running process:

dotnet dump collect -p 123
