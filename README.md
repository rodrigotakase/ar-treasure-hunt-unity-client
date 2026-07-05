# AR Treasure Hunt

A mobile AR treasure hunt game built with Unity and AR Foundation. Crystal treasures are hidden in the real world, each labelled with a printed QR code. Players scan a QR with the in-app camera, a crystal appears in AR, and tapping it until it shatters collects the treasure and reveals its location. Progress and a leaderboard are synced with a backend API.

<p align="center">
  <img src="Docs/app.gif" width="300" alt="App demo">
</p>

## How it plays

1. **Scan** - point the camera at a treasure QR code. The app reads the treasure id from the QR and fetches its info from the backend.
2. **Shatter** - a floating crystal appears in AR, tinted with the treasure's color. Tap it repeatedly to fracture it, with a progress bar counting the taps.
3. **Collect** - on the final tap the crystal bursts into sparks, the collection is registered on the server, and a popup reveals the treasure's real-world location. Already-collected treasures show a friendly reminder instead of double-counting.
4. **Track** - the Treasures tab lists every treasure in the hunt with hints and found status; the Ranks tab shows the live leaderboard with your nickname.

## Tech

- **Unity 6** (URP) targeting iOS and Android, portrait
- **AR Foundation** - camera feed, plane detection, ARKit/ARCore
- **ZXing.Net** - QR decoding on a background thread from AR camera CPU images
- **REST backend** - anonymous player identity via a device-generated UUID, no signup

## Project layout

```
Assets/ARTreasureHunt/
├── Scenes/            main scene
├── Scripts/           gameplay, UI controllers, API client
├── Prefabs/           CrystalPrefab, AR plane visual
├── Textures/ SFX/ Fonts/ Materials/ 3DModels/
```

## Backend

The app talks to a small REST API:

- `GET /api/treasures/{id}` - treasure info after a QR scan
- `POST /api/collect` - registers a collection (idempotent)
- `GET /api/hunts/{id}/treasures` - hunt checklist with found status
- `GET /api/hunts/{id}/leaderboard` - ranked players

Players are identified by a UUID generated on first run and stored on the device. Nicknames are optional and sent along with collect requests.

## Running it

1. Open the project in Unity 6 with iOS/Android build support.
2. Open the scene in `Assets/ARTreasureHunt/Scenes/`.
3. Build to a device (AR + camera requires real hardware; the editor's XR Simulation shows the UI and planes but has no QR codes to scan).
4. Print QR codes that encode the client URL with the treasure id as the final part, e.g. `https://<client-url>?treasure=<uuid>`.

## Credits

- Crystal shader: **Translucent Crystals** by SineVFX
- Shatter particles: **Cartoon FX Remaster** by Jean Moreno (JMO)
- Crystal model: *Fantasy Stylized Crystals*
- Font: **Space Grotesk**
- SFX from Freesound: [790466](https://freesound.org/s/790466/) by omiranda14, [444136](https://freesound.org/s/444136/) by lurpsis
