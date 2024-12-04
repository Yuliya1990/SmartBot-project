using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using SmartBots.CognitiveModels;

namespace SmartBots.Bots
{
    public class SmartBot : ActivityHandler
    {
        private readonly SmartBotRecognizer _cluRecognizer;

        public SmartBot(SmartBotRecognizer cluRecognizer) : base()
        {
            _cluRecognizer = cluRecognizer;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var cluResult = await _cluRecognizer.RecognizeAsync<SmartModel>(turnContext, cancellationToken);
            switch (cluResult.GetTopIntent().intent)
            {
                case SmartModel.Intent.Llamar:
                    var llamarContact = cluResult.Entities.GetContact();
                    string llamarText;

                    if (llamarContact == null)
                        llamarText = "No me ha sido posible detectar el contacto en tu frase";
                    else
                        llamarText = "Entendido, voy a llamar a " + llamarContact;

                    var llamarMessage = MessageFactory.Text(llamarText, llamarText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(llamarMessage, cancellationToken);
                    break;

                case SmartModel.Intent.EnviarMensaje:
                    var mensajeContact = cluResult.Entities.GetContact();
                    var mensajeFrase = cluResult.Entities.GetPhrase();
                    string mensajeText;

                    if (mensajeContact == null)
                        mensajeText = "No me ha sido posible detectar el contacto en tu frase";
                    else if (mensajeFrase == null)
                        mensajeText = "No me ha sido posible detectar el mensaje en tu frase";
                    else
                        mensajeText = string.Format("Entendido, voy a enviar \"{0}\" a {1}", mensajeFrase, mensajeContact);

                    var enviarMessage = MessageFactory.Text(mensajeText, mensajeText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(enviarMessage, cancellationToken);
                    break;

                case SmartModel.Intent.PonerDespertador:
                    var time = cluResult.Entities.GetTime();
                    string timeText;

                    if (time == null)
                        timeText = "No me ha sido posible detectar el tiempo en tu frase";
                    else
                        timeText = "Entendido, voy a poner despertador a las " + time;

                    var timeMessage = MessageFactory.Text(timeText, timeText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(timeMessage, cancellationToken);
                    break;

                case SmartModel.Intent.BuscarEnInternet:
                    var buscarFrase = cluResult.Entities.GetPhrase();
                    string buscarText;

                    if (buscarFrase == null)
                        buscarText = "No me ha sido posible detectar el frase para buscar";
                    else
                        buscarText = string.Format("Entendido, voy a buscar \"{0}\" por Internet", buscarFrase);

                    var buscarMessage = MessageFactory.Text(buscarText, buscarText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(buscarMessage, cancellationToken);
                    break;

                case SmartModel.Intent.PonerCanción:
                    var cancion = cluResult.Entities.GetSong();
                    string cancionText;

                    if (cancion == null)
                        cancionText = "No me ha sido posible detectar la canción en tu frase";
                    else 
                        cancionText = "Entendido, voy a poner canción " + cancion;

                    var cancionMessage = MessageFactory.Text(cancionText, cancionText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(cancionMessage, cancellationToken);
                    break;
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Perdona, no entendí lo que me pediste. Preguntame de otra forma";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, 
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hola, Bienvenido a SmartBot! " +
                "Con la ayuda de este bot, puedes llamar, escribir mensajes, configurar una alarma " +
                "y buscar información en Internet. Sólo di el comando que quieres hacer";

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

    }
}
