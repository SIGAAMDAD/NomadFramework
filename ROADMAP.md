# ROADMAP

## Additional Features
### Nomad.Core
- **Custom Container Implementations with Custom Allocation Routines**: Currently the framework utilizes the .NET runtime standard containers. And while those work wonderfully, they also do not offer complete control over how the memory is managed/allocated. For absolute maximum performance among specific edge cases, and to improve allocation throughput, there will be custom containers that can are essentially replicas of the original containers but with custom allocation and QoL extension methods.
### Nomad.Save
- **Corruption recovery**:
- **Version migration**:
### Nomad.FileSystem
- **Stream allocation strategies**: Allows the user to specify how they want a stream's data allocated & stored. Whether that be through a stack-only span, an array pool, or just the default. This is a low-level feature aimed at increasing performance and CPU efficiency for power users.
- **File watcher**:
- **Memory mapped streams**: Streams can be mapped into memory for much faster reads & writes at the cost of higher memory usage. This is effectively like using the MemoryFile stream type, but much more low level.
- **File Opening Configuration**: Currently the configurations for opening files is extremely limited, I plan to expand it so that we can have many more options around general file handling
### Nomad.Events
- Event recorders
- Event priority queues
## Upcoming Subsystems
### Epic & GoG Support
### Unreal C++ Bindings
### MonoGame Support
## Fixes
* Implement AtomicSubscriptionSet
* Finish SubscriptionSetBase
* Optimize PumpAsync using an ObjectPool, we're allocating a new array each pump
* Implement EventQueue
* Implement EventPriority to priority sorting/pumping

