namespace TCG.Core.Dialogues
{
    public class TextCommandsFactory
    {
        public TextCommand CreateCommand(string commandName)
        {
            switch (commandName) {
                case "pause":
                case "p":
                    return new TextCommandPause();
                case "camshake": return new TextCommandCameraShake();
                case "spriteshake": return new TextCommandSpriteShake();
                case "textshake": return new TextCommandTextShake();
                case "rdmpause": return new TextCommandRdmPause();
                case "rdmp": return new TextCommandRdmPause();
                case "squiggle": return new TextCommandSquiggle();
               //case "material": return new TextCommandTextMaterial();
               //case "fadein": return new TextCommandTextAppear();
                case "mirage": return new TextCommandMirage();
            }

            return null;
        }
    }
}