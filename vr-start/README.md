# TimeBite VR Start

Unity 6 LTS Quest scaffold for the TimeBite Meta VR Start workstream.

## Target

- Unity 6 LTS
- Android, ARM64
- Meta Quest, hands-only interaction
- Latest stable Meta XR SDK with Interaction SDK imported from the Unity Package Manager or Meta scoped registry
- Unity OpenXR: Meta enabled for Quest runtime support

## Scenes

- `Assets/Scenes/TaskDashboard.unity` is the startup scene placeholder.
- `TaskDashboardController` builds task rows from mocked editor data or backend data.
- `TimeBiteApiClient` talks to `/health`, `/tasks`, and `/process` using a `TimeBiteApiConfig` ScriptableObject base URL.

## Setup Notes

1. Open `vr-start/` in Unity 6 LTS.
2. Import the latest stable Meta XR SDK and Interaction SDK. As of September 30, 2026, Meta's public Asset Store listing shows the Meta XR SDK updated on September 23, 2026.
3. Assign `Assets/TimeBite/Config/TimeBiteApiConfig.asset` to the dashboard/API client.
4. Keep `Use Mock Data In Editor` enabled for offline editor play mode.
5. For device builds, set the API base URL to a network-reachable backend, not `localhost`.

## Acceptance Checks

- The project settings target Android ARM64 and Quest-compatible XR.
- The dashboard scripts are hands-first and expose poke/far-ray interaction affordance hooks.
- API payloads match the FastAPI backend shapes: `/process` accepts task/text plus telemetry fields; `/tasks` returns `id`, `title`, `priority`, `status`; `/health` returns backend status.
- Telemetry posts CYRA-shaped attempt, latency, correction, spatial-action, and accuracy fields back through `/process`.
