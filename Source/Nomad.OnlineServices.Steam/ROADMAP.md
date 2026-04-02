# ROADMAP

## What Mature Steam Service Layers Usually Cover

- Initialization, callback dispatch, achievements, stats, cloud storage, and
  lobby or matchmaking support.
- Clear abstractions around asynchronous Steam callbacks so gameplay code does
  not become callback-shaped.
- Good failure handling when Steam is unavailable, the user is offline, or the
  app is running outside a valid Steam context.
- A clean seam for swapping Steam-specific features out of builds that do not
  ship with Steam.

## Suggested Next Steps

- Clarify the public contract for lobbies, members, matchmaking, stats, and
  cloud storage with more examples and usage notes.
- Add lifecycle diagrams for bootstrapping, callback polling, and teardown.
- Add compatibility shims or no-op stubs for non-Steam builds that still link
  against higher-level gameplay systems.
- Replace the generic package README with Steam-specific bootstrap and auth
  guidance.

## Bugfixes And Hardening

- Implement the pending map and gamemode filtering noted in
  `Private/Services/LobbyServices/SteamLobbyLocator.cs`.
- Rename the `Private/Repostories` folder to `Private/Repositories` to remove a
  typo that will keep leaking into tooling and docs.
- Expand tests for callback dispatch ordering, lobby membership changes, and
  cloud-storage failure paths.
- Decide whether this package's metadata and docs should explicitly describe its
  engine compatibility limits.

## Future Additions

- Rich presence, invites, and relay networking support.
- Cross-save coordination with the save subsystem.
- Anti-abuse and moderation helpers for lobbies and chat.
