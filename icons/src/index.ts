import icons from "./icons.json" with { type: "json" };

const browserIcons = createLookup("browsers", icons.browsers);
const osIcons = createLookup("os", icons.os);

export function getBrowserIcon(name: string): string | undefined {
  return browserIcons.get(name);
}

export function getOsIcon(name: string): string | undefined {
  return osIcons.get(name);
}

function createLookup(directory: string, groups: Record<string, string[]>): Map<string, string> {
  return new Map(
    Object.entries(groups).flatMap(([file, names]) =>
      names.map((name) => [name, `${directory}/${file}`] as const),
    ),
  );
}
