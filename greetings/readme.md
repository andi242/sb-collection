# greetings

simple greetings on first-words event, saved in a user variable.
On first words the bot replies with the saved message. Replies can not invoke `!commands`.

Commands:
- `!greet set <msg>` set the greeting for the user.
- `!greet del` delete the greetings.

## ToDo

- command filter to prevent / commands in greetings
  - `if regex '^/.*', err, return false;`