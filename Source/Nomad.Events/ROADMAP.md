# ROADMAP

## What Mature Event Systems Usually Cover

- Synchronous and queued dispatch with explicit ordering guarantees.
- Safe subscription lifetimes, async-aware listeners, and low-allocation hot
  paths.
- Diagnostics that explain who raised an event, who consumed it, and where
  listener failures occurred.
- Clear threading rules so gameplay code knows which event types may cross
  threads.

## Suggested Next Steps

- Finish the event queue story so priorities, ordering, and scheduled dispatch
  are first-class rather than partially implemented.
- Document the intended difference between the current subscription set
  implementations and when each should be used.
- Add observability for listener counts, queue depth, dropped events, and slow
  handlers.
- Replace the generic package README with docs focused on lifetime management
  and threading.

## Bugfixes And Hardening

- Implement `Private/SubscriptionSets/AtomicSubscriptionSet.cs`.
- Consolidate duplicated behavior called out in
  `Private/SubscriptionSets/SubscriptionSetBase.cs`.
- Finish the pending catch or bubbler counter work noted in
  `Private/SubscriptionSets/SubscriptionSet.cs`.
- Add focused tests for queue priority ordering, async listener failure paths,
  and concurrent subscribe or unsubscribe behavior.

## Future Additions

- Event replay and tracing for debugging.
- Sticky or retained events for stateful systems that need late subscribers.
- Cross-process or network event bridges for multiplayer or tooling scenarios.
