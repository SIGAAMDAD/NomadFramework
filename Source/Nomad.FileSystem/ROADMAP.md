# ROADMAP

## What Mature Game File Systems Usually Cover

- Abstracted read and write streams, layered search paths, and predictable mount
  precedence.
- Cross-platform path normalization, atomic writes, temp storage, and directory
  utilities.
- Asset discovery helpers, watch services, and archive support for shipped
  content.
- Good failure reporting for missing files, permission issues, and partial
  writes.

## Suggested Next Steps

- Flesh out the VFS vision with explicit mount tables, archive mounts, and
  search-order rules.
- Add file watching and persistent indexing to improve editor workflows and
  startup time.
- Add configurable temp-cache retention and cleanup behavior.
- Replace the generic package README with focused VFS and stream usage docs.

## Bugfixes And Hardening

- Fix the metadata copy gap called out in
  `Private/Services/FileSystemService.cs` so copy operations preserve more than
  raw contents.
- Expand tests for path normalization, file permissions, and platform-specific
  edge cases.
- Revisit the commented TODO in `Nomad.FileSystem.Tests` around file format or
  write options so the API surface stays ahead of future archive work.

## Future Additions

- Memory-mapped stream support for large-file workflows.
- Native `.zip`, `.pak`, or bundle readers.
- Async change notifications for tooling and hot reload.
