# Define the configuration
CONFIGURATION ?= Release

# Define the commands for running the specific benchmarks
VECTOR3_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*Vector3Benchmark*'
VECTOR_OPERATIONS_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorOperationsBenchmark*'
VECTOR_INTRINSICS_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorIntrinsicsBenchmark*'
VECTOR_TRICKS_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorTricksBenchmark*'
VECTOR_PRIMES_BENCHMARK_CMD = dotnet run -c $(CONFIGURATION) --filter '*VectorPrimesBenchmark*'

# Default target (runs all benchmarks)
.PHONY: all
all: vector3-benchmark vector-operations-benchmark vector-intrinsics-benchmark vector-tricks-benchmark vector-primes-benchmark

# Run the Vector3Benchmark
.PHONY: vector3-benchmark
vector3-benchmark:
	@echo "Running Vector3Benchmark..."
	$(VECTOR3_BENCHMARK_CMD)

# Run the VectorOperationsBenchmark
.PHONY: vector-operations-benchmark
vector-operations-benchmark:
	@echo "Running VectorOperationsBenchmark..."
	$(VECTOR_OPERATIONS_BENCHMARK_CMD)

# Run the VectorIntrinsicsBenchmark
.PHONY: vector-intrinsics-benchmark
vector-intrinsics-benchmark:
	@echo "Running VectorIntrinsicsBenchmark..."
	$(VECTOR_INTRINSICS_BENCHMARK_CMD)

# Run the VectorTricksBenchmark
.PHONY: vector-tricks-benchmark
vector-tricks-benchmark:
	@echo "Running VectorTricksBenchmark..."
	$(VECTOR_TRICKS_BENCHMARK_CMD)

# Run the VectorTricksBenchmark
.PHONY: vector-primes-benchmark
vector-primes-benchmark:
	@echo "Running VectorPrimesBenchmark..."
	$(VECTOR_PRIMES_BENCHMARK_CMD)

# Clean build artifacts (optional)
.PHONY: clean
clean:
	@echo "Cleaning project..."
	dotnet clean
