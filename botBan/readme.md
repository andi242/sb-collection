# botBan

Basically a word-list-ban. Checks a curated list of words and bans user, if first-words-message the word list is hit.  
That User gets timeouted to remove chat messages and banned from the channel.

Known users can be placed in a group to prevent accidential bans.  
Also all banned users are added to an extra group in SB.

Commands:
- `!ban add <keyword>` adds a keyword to the list
- `!ban list` lists blacklisted keywords
- `!ban del` or `!ban remove` lists the current blocked terms
- `!ban del <keyword>` or `!ban remove <keyword>` removes a keyword from the list
