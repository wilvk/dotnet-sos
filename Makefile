.PHONY: llvm list d-build-debug d-llvm-code-line-breakpoint d-llvm-method-breakpoint d-build-netcoredbg

NETCOREDBG_BUILD_PTH ?= $(NETCOREDBG_PTH)/build
NETCOREDBG_OUT_PTH ?= $(NETCORE_BUILD_PTH)/src/debug/netcoredbg
LIBDBGSHIM_PTH ?= $(NETCOREDBG_PTH)/.dotnet/shared/Microsoft.NETCore.App/2.1.16/libdbgshim.so

define DOCKER_COMPOSE_LLVM
	docker-compose -f docker/docker-compose-llvm.yml build
	docker-compose -f docker/docker-compose-llvm.yml run --rm --service-ports
endef

define DOCKER_COMPOSE_GDB
	docker-compose -f docker/docker-compose-gdb.yml build
	docker-compose -f docker/docker-compose-gdb.yml run --rm --service-ports
endef

llvm:
	$(DOCKER_COMPOSE_LLVM) llvm /bin/bash

dotnet-build:
	docker build -f ./docker/Dockerfile-dotnet-build --memory="4096m" --memory-swap="8192m" -t dotnet-sos .
	docker run -it -v $(shell pwd):/work --privileged dotnet-sos /bin/bash

list:
	@cat Makefile| grep -E '^(.*):$$'|sed -e 's/://g'

d-build-debug:
	dotnet build ./source/own/test_debug/

d-llvm-code-line-breakpoint:
	lldb --source ./scripts/startup.lldb --source ./scripts/code_line_breakpoint.lldb

d-llvm-method-breakpoint:
	lldb --source ./scripts/startup.lldb --source ./scripts/method_breakpoint.lldb

gdb-start-server:
	gdbserver :9999 /work/tests/binaries/hello_linux

gdb:
	$(DOCKER_COMPOSE_GDB) dotnet-gdb /bin/bash

d-build-netcoredbg-310:
	NETCOREDBG_PTH=/work/source/own/netcoredbg310 $(MAKE) d-build-netcoredbg

d-build-netcoredbg-orig:
	NETCOREDBG_PTH=/work/source/cloned/netcoredbg $(MAKE) d-build-netcoredbg

d-build-netcoredbg:
	cd $(NETCOREDBG_PTH) && rm -rf ./build || true && mkdir build
	cd $(NETCOREDBG_BUILD_PTH) && CC=clang CXX=clang++ cmake .. -DCMAKE_INSTALL_PREFIX=$(shell pwd)/../bin
	cd $(NETCOREDBG_BUILD_PTH) && make
	cd $(NETCOREDBG_OUT_PTH) && cp $(LIBDBGSHIM_PTH) .
