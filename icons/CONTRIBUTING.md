# Contributing

`src/icons.json` maps every icon file to the names that use it. When UaDetector detects a new browser
or operating system, either add its name to an existing icon, such as `harmonyos.png` for HarmonyOS NEXT,
or add a new icon:

1. Find the largest source available, ideally an SVG or an image of at least 128px, such as an app
   store icon or an official logo. Prefer a version with a transparent background. Don't use
   GPL-licensed images.
2. Resize it to a 48×48 PNG. Don't otherwise edit the logo: don't remove or add a background,
   recolor or sharpen it. This needs [ImageMagick](https://imagemagick.org) 7, and
   [librsvg](https://wiki.gnome.org/Projects/LibRsvg) for SVGs:

   ```bash
   # PNG, JPG or GIF
   magick logo.png -background none -resize 48x48 -gravity center -extent 48x48 -strip PNG32:browsers/example.png

   # SVG: render it large first, then resize the PNG as above
   rsvg-convert -h 512 logo.svg -o logo.png

   # ICO: list the frames, then extract the largest one by its index and resize the PNG as above
   magick identify favicon.ico
   magick "favicon.ico[0]" logo.png

   # Logo with the product name next to it: crop it to the symbol, given as WIDTHxHEIGHT+X+Y
   # in pixels, trim the empty space around it, then resize the PNG as above
   magick logo.png -crop 512x512+0+0 +repage -trim +repage logo.png
   ```

3. Add the file name and the names that use it to `src/icons.json`.
4. Record where the image came from in [SOURCES.md](SOURCES.md). If its license requires attribution,
   such as CC BY, also add it to [NOTICE.md](NOTICE.md).

A test in `UaDetector.Tests` checks that every name exists in UaDetector, that no name appears twice,
that every referenced file exists and that every icon is 48×48.
