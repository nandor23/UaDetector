# UaDetector Icons

Browser and operating system icons for the names returned by [UaDetector](https://github.com/nandor23/UaDetector).

## Installation

```bash
npm install @uadetector/icons
```

## Usage

`getBrowserIcon` and `getOsIcon` take a name exactly as UaDetector returns it and return the icon's
path relative to the package root, or `undefined` when there is no icon for the name.

```ts
import { getBrowserIcon, getOsIcon } from "@uadetector/icons";

getBrowserIcon("Whale Browser"); // "browsers/whale-browser.png"
getOsIcon("HarmonyOS NEXT");     // "os/harmonyos.png"
getBrowserIcon("Not A Browser"); // undefined
```

Not every browser and operating system has an icon, so handle `undefined` with a fallback of your own.

The returned path points into the package's `browsers` and `os` folders. Serve those folders with your
static assets, or load the icons from a CDN:

```ts
const iconUrl = `https://cdn.jsdelivr.net/npm/@uadetector/icons@<version>/${icon}`;
```

## Contributing

To add an icon or map a name to an existing one, see
[CONTRIBUTING.md](https://github.com/nandor23/UaDetector/blob/main/icons/CONTRIBUTING.md).

## License

The code and the mapping are licensed under the
[MIT License](https://github.com/nandor23/UaDetector/blob/main/icons/LICENSE). The icons are trademarks
of their respective owners and are not covered by it. See
[NOTICE.md](https://github.com/nandor23/UaDetector/blob/main/icons/NOTICE.md).
