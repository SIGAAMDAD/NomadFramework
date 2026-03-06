# ROADMAP

## Additional Features
### Nomad.Core
- **Custom Container Implementations with Custom Allocation Routines**: Currently the framework utilizes the .NET runtime standard containers. And while those work wonderfully, they also do not offer complete control over how the memory is managed/allocated. For absolute maximum performance among specific edge cases, and to improve allocation throughput, there will be custom containers that can are essentially replicas of the original containers but with custom allocation and QoL extension methods.
### Nomad.Save
- **Corruption recovery**:
- **Version migration**:
### Nomad.FileSystem
- **File watcher**:
- **Temporary Cache Purging**: Deleting temporary files older than a config var's set value.
- **Memory mapped streams**: Streams can be mapped into memory for much faster reads & writes at the cost of higher memory usage. This is effectively like using the MemoryFile stream type, but much more low level.
- **Archive Handling**: Reading archived streams like .zip, .pak, etc.
- **File Indexing**: Persistent file index cache for faster subsequent startups
### Nomad.Events

## Upcoming Subsystems
### Epic & GoG Support
### Unreal C++ Bindings
### MonoGame Support