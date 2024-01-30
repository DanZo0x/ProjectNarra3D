using System.Globalization;
using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandTextMaterial: TextCommand
    {
        private string _material;
        

        public override bool AlwaysUpdated => true;

        public override bool NeedsCacheTextMesh => true;

        public override bool NeedsClosingTag => true;

        
        public override void SetupData(string strCommandData)
        {
            _material = "Fonts & Materials/"+strCommandData;
        }

        public override void OnUpdate()
        {
            for (int i = EnterIndex; i <= ExitIndex; ++i)
            {
                //ChangeTexture(_material,i);
            }
        }
    }
}