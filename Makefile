.PHONY: llvm list d-build-debug d-llvm-code-line-breakpoint d-llvm-method-breakpoint

define DOCKER_COMPOSE_LLVM
	docker-compose -f docker-compose-test-llvm.yml build
	docker-compose -f docker-compose-test-llvm.yml run --rm --service-ports
endef

llvm:
	$(DOCKER_COMPOSE_LLVM) test-llvm-server /bin/bash

diagnostics:
	$(DOCKER_COMPOSE_LLVM) diagnostics /bin/bash

list:
	@cat Makefile| grep -E '^(.*):$$'|sed -e 's/://g'

d-build-debug:
	dotnet build ./test_debug/

d-llvm-code-line-breakpoint:
	lldb --source ./scripts/startup.lldb --source ./scripts/code_line_breakpoint.lldb

d-llvm-method-breakpoint:
	lldb --source ./scripts/startup.lldb --source ./scripts/method_breakpoint.lldb
