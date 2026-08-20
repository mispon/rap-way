# Engine-Independent Projects

The projects in this directory compile the same Domain and Application source files used by Unity. They do not contain duplicate gameplay source.

## Run tests

From the repository root:

```powershell
dotnet test RapWay.slnx --configuration Release
```

The SDK version is pinned by `global.json` and test package versions are pinned in the test project.

## Test conventions

- Keep tests deterministic and independent of Unity, network access, wall-clock time, execution order, and production save data.
- Name tests after observable behavior; use Arrange/Act/Assert separation only when it improves readability.
- Add boundary, failure, and invariant cases for state-changing rules instead of testing only a happy path.
- A logic bug should receive a reproducing test before or with its fix when practical.
- Architecture tests must continue to reject Unity and vendor references from Domain and Application.
- Unity EditMode or PlayMode tests belong in Unity-only test assemblies when the behavior truly requires Unity APIs or scene wiring.
