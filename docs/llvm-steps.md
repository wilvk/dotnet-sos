# Steps for getting a llvm terminal with test_app

from the command line:

$ make llvm

- you will enter the container

$ make d-build-debug

$ make d-llvm-method-breakpoint


bpmd:

bpmd does not accept offsets nor parameters in the method name. You can add
an IL offset as an optional parameter separate from the name. If there are overloaded
methods, bpmd will set a breakpoint for all of them.
