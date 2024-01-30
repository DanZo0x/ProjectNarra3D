using System.Collections.Generic;

namespace TCG.Core.Dialogues
{
    public static class TextCommandUtils
    {
        public static TextCommand[] FindAlwaysUpdatedCommands(TextCommand[] commands)
        {
            List<TextCommand> resultList = new List<TextCommand>();
            foreach (TextCommand command in commands) {
                if (!command.AlwaysUpdated) continue;
                resultList.Add(command);
            }

            return resultList.ToArray();
        }

        public static TextCommand[] FindCommandsToEnter(TextCommand[] commands, int startIndex, int endIndex)
        {
            List<TextCommand> resultList = new List<TextCommand>();
            foreach (TextCommand command in commands) {
                if (command.EnterIndex < startIndex) continue;
                if (command.EnterIndex > endIndex) continue;
                resultList.Add(command);
            }

            return resultList.ToArray();
        }

        public static TextCommand[] FindCommandsToExit(TextCommand[] commands, int startIndex, int endIndex)
        {
            List<TextCommand> resultList = new List<TextCommand>();
            foreach (TextCommand command in commands) {
                if (command.ExitIndex < startIndex) continue;
                if (command.ExitIndex > endIndex) continue;
                resultList.Add(command);
            }

            return resultList.ToArray();
        }
    }
}