# Post Office: The Letter Filter - A RimWorld Mod

Post Office is a RimWorld mod that allows you to mute or filter specific types of letters and messages, including notifications like "Legendary Work" or "Raid: [some faction]". This helps maintain immersion and surprise, especially when playing with mods like "CAI 5000 - Advanced AI + Fog Of War".

## Features

At its core, Post Office works like a firewall for in-game letters and messages, allowing you to allow/deny messages based on configurable rules.

- Globally disable any letter type (e.g., "ThreatBig", "NewQuest") from all mods.
- Use regular expressions (.NET flavor) to filter messages based on their content. See this [sample on regex101](https://regex101.com/r/ajvOBS/1) to see how it works and test your own regex patterns.
- Compatible with save games, so can be added or removed at any time.
- Works with letters from all mods, thanks to regex filtering.
- Should be compatible with most mods, but if you find any issues, please report them.