# Contributing

`src/icons.json` maps every icon file to the names that use it. When UaDetector detects a new browser
or operating system, either add its name to an existing icon, such as `harmonyos.png` for HarmonyOS NEXT,
or add a new icon:

1. Find the product's official logo, as large as possible, ideally an SVG or an image of at least 128px.
   Look on Wikimedia Commons, the product's website or brand assets, its GitHub organization avatar,
   or its App Store or Google Play listing. Prefer a version with a transparent background, and prefer
   the symbol over a wide wordmark, which is unreadable at 48px. If the product has no logo of its own,
   use the maker's logo only when the product ships under that brand, such as Oracle for Oracle Linux.
   Don't use GPL- or LGPL-licensed images.
2. Resize it to a 48×48 PNG. Don't otherwise edit the logo: don't remove or add a background,
   recolor or sharpen it. This needs [ImageMagick](https://imagemagick.org) 7, and
   [librsvg](https://wiki.gnome.org/Projects/LibRsvg) for SVGs:

   ```bash
   # PNG, JPG or GIF
   magick logo.png -background none -resize 48x48 -gravity center -extent 48x48 -strip PNG32:browsers/example.png

   # SVG: render it large first, then resize the PNG as above. Use -w 512 instead for a wide logo
   rsvg-convert -h 512 logo.svg -o logo.png

   # ICO: list the frames, then extract the largest one by its index and resize the PNG as above
   magick identify favicon.ico
   magick "favicon.ico[0]" logo.png

   # Logo with the product name next to it: crop it to the symbol, given as WIDTHxHEIGHT+X+Y
   # in pixels, trim the empty space around it, then resize the PNG as above
   magick logo.png -crop 512x512+0+0 +repage -trim +repage logo.png
   ```

   Check the result on a light and a dark background before adding it.

3. Add the file name and the names that use it to `src/icons.json`.
4. Record where the image came from in [SOURCES.md](SOURCES.md). Use the license of the source, or
   "Brand asset" for an image taken from the owner, such as their website or app store listing. For an
   image on Wikimedia Commons, use the license on its file page even if it looks mislabeled, since
   following it costs nothing. If the license requires attribution, such as CC BY or MIT, also add it to
   [NOTICE.md](NOTICE.md). Licenses that must be shipped in full, such as MIT, BSD or Apache, also need
   the icon's copyright notice in its NOTICE.md entry and the license text in [LICENSES](LICENSES).

A test in `UaDetector.Tests` checks that every name exists in UaDetector, that no name appears twice,
that every referenced file exists and that every icon is 48×48.
