- [x] get MdDumper working for a dotnetcore3.1 app ( currently only .net fx)
- [ ] add mddumper build and example to makefile in docker file
- [ ] get netcoredbg building in a docker file and add to mkfile
- [ ] look into writing a gdb plugin for linking the symbols to the addresses
    https://docs.kde.org/stable5/en/applications/kate/kate-application-plugin-gdb.html


- [ ] read this article: https://benhall.io/c-debug-vs-release-builds-and-debugging-in-visual-studio-from-novice-to-expert-in-one-blog-article/

- [ ] put the repl project into an application: /app/diagnostics/src/Microsoft.Diagnostics.Repl

- [ ] look into ICorDebug and IUnknown interfaces and MIDL in dotnet core:
      /app/diagnostics/src/pal/prebuilt/inc/cordebug.h
      /app/diagnostics/src/inc/icordebug.idl
