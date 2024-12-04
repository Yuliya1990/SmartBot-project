using Microsoft.Bot.Builder;
using Microsoft.BotBuilderSamples.Clu;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SmartBots.CognitiveModels
{
    public class SmartModel : IRecognizerConvert
    {
        public enum Intent
        {
            Llamar,
            EnviarMensaje,
            PonerDespertador,
            BuscarEnInternet,
            PonerCanción,
            None
        }

        public string Text { get; set; }

        public string AlteredText { get; set; }

        public Dictionary<Intent, IntentScore> Intents { get; set; }

        public CluEntities Entities { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var jsonResult = JsonConvert.SerializeObject(result, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var app = JsonConvert.DeserializeObject<SmartModel>(jsonResult);

            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) GetTopIntent()
        {
            var maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }

            return (maxIntent, max);
        }

        public class CluEntities
        {
            public CluEntity[] Entities;

            public CluEntity[] GetContacts() => Entities.Where(e => e.Category == "Contacto").ToArray();
            public CluEntity[] GetTimes() => Entities.Where(e => e.Category == "Tiempo").ToArray();
            public CluEntity[] GetPhrases() => Entities.Where(e => e.Category == "Frase").ToArray();
            public CluEntity[] GetSongs() => Entities.Where(e => e.Category == "Canción").ToArray();

            public string GetContact() => GetContacts().FirstOrDefault()?.Text;
            public string GetTime() => GetTimes().FirstOrDefault()?.Text;
            public string GetPhrase() => GetPhrases().FirstOrDefault()?.Text;
            public string GetSong() => GetSongs().FirstOrDefault()?.Text;
        }
    }
}
