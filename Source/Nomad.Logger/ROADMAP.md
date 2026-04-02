# ROADMAP

## What Mature Logging Subsystems Usually Cover

- Structured messages, categories, levels, sinks, and runtime filtering.
- Low-overhead logging on hot paths, with batching or async flushing where
  needed.
- File rotation, retention, and failure-safe behavior when a sink cannot write.
- Easy bridging into engine consoles, test output, and external telemetry.

## Suggested Next Steps

- Promote the logger from string-focused output toward richer structured payloads
  and named fields.
- Add sink composition so file, console, and engine debug sinks can be layered
  predictably.
- Add category and scope guidance for subsystem authors so logs stay consistent
  across the framework.
- Replace the generic package README with module-specific setup and sink docs.

## Bugfixes And Hardening

- Add a dedicated `Nomad.Logger.Tests` project for sink failure handling,
  formatting, level filtering, and concurrent writers.
- Verify that file sink errors degrade cleanly instead of breaking callers that
  only wanted best-effort diagnostics.
- Add tests around category naming and message formatting stability to prevent
  log churn between releases.

## Future Additions

- Rolling file retention and size-based rotation.
- OTLP, Serilog, or other external telemetry adapters.
- An in-engine log viewer with filtering and subsystem health summaries.
