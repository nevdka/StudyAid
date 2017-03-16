using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyAid
{
    class TextProvider
    {
        private string text;
        private ConfigSettings _config;
        private List<string> messages;
        private Random rand = new Random();

        public TextProvider( ConfigSettings config )
        {
            _config = config;

            text = config.TextFileName;
            messages = new List<string>( File.ReadAllLines( config.TextFileName ) );
        }

        public string GetNextText()
        {
            int index = rand.Next( messages.Count );
            return messages[index];
        }

        public string GetNextText( int percent )
        {
            return $"{percent}% done!";
        }
    }
}
