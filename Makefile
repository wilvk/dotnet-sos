.PHONY: llvm list d-build-debug d-llvm-code-line-breakpoint d-llvm-method-breakpoint

define DOCKER_COMPOSE_LLVM
	docker-compose -f docker-compose-test-llvm.yml build
	docker-compose -f docker-compose-test-llvm.yml run --rm --service-ports
endef

define DOCKER_COMPOSE_GDB
	docker-compose -f docker-compose-test-gdb.yml build
	docker-compose -f docker-compose-test-gdb.yml run --rm --service-ports
endef

llvm:
	$(DOCKER_COMPOSE_LLVM) test-llvm-server /bin/bash

dotnet-build:
	docker build -f Dockerfile-dotnet-build --memory="4096m" --memory-swap="8192m" -t dotnet-sos .
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
