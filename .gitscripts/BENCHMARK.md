# The Benchmark for Filter Scripts

The command `git add --renormalize .` allow us to perform a full re-indexing of the repository.

To measure the elapsed time, the following command was executed in PowerShell.

```pwsh
Measure-Command {start-process git -argumentlist "add --renormalize ." -wait}
```

| Script                   | Elapse (ms)   | Commit   | HW Spec                                         |
| ------------------------ | ------------- | -------- | ----------------------------------------------- |
| filter_usharp.py         | 5013.8651     | 1cf9182a | CPU: AMD Ryzen 9 9950X (with ~10% initial load) |
| filter_usharp_process.py | **1010.4108** | 1cf9182a | CPU: AMD Ryzen 9 9950X (with ~10% initial load) |
