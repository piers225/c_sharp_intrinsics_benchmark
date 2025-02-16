# Define the configuration
CONFIGURATION ?= Release

# Define the commands for running the specific benchmarks
VECTOR3_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*Vector3Benchmark*'
VECTOR_ADD_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorAddBenchmark*'
VECTOR_INTRINSICS_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorIntrinsicsBenchmark*'

# Default target (runs all benchmarks)
.PHONY: all
all: vector3-benchmark vector-add-benchmark vector-intrinsics-benchmark

# Run the Vector3Benchmark
.PHONY: vector3-benchmark
vector3-benchmark:
	@echo "Running Vector3Benchmark..."
	$(VECTOR3_BENCHMARK_CMD)

# Run the VectorAddBenchmark
.PHONY: vector-add-benchmark
vector-add-benchmark:
	@echo "Running VectorAddBenchmark..."
	$(VECTOR_ADD_BENCHMARK_CMD)

# Run the VectorIntrinsicsBenchmark
.PHONY: vector-intrinsics-benchmark
vector-intrinsics-benchmark:
	@echo "Running VectorIntrinsicsBenchmark..."
	$(VECTOR_INTRINSICS_BENCHMARK_CMD)

# Clean build artifacts (optional)
.PHONY: clean
clean:
	@echo "Cleaning project..."
	dotnet clean
