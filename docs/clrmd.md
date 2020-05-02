
clrmd tutorial:

https://github.com/microsoft/clrmd/blob/master/doc/GettingStarted.md

invasive, non-invasive and passive attach to process modes:

To be clear though, the difference between an invasive and non-invasive attach doesn't matter to CLRMD. It only matters if you need to control the process through the IDebug interfaces. If you do not care about getting debugger events or breaking/continuing the process, you should choose a non-invasive attach.
