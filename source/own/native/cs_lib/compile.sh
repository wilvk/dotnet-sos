gcc -g -c test.c -o test.o                                                      # compiling a C library
g++ -dynamiclib -undefined suppress -flat_namespace test.o -o libtest.dylib     # generate a dynamic library from our object file
dotnet build                                                                    # compiling our C# application
