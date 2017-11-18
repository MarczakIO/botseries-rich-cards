using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

[Serializable]
public class MainDialog : IDialog<object>
{
    private const string CarouselCard = "Carousel card";
    private const string HeroCard = "Hero card";
    private const string ThumbnailCard = "Thumbnail card";
    private const string ReceiptCard = "Receipt card";
    private const string SigninCard = "Sign-in card";
    private const string VideoCard = "Video card";
    private const string AudioCard = "Audio card";

    private List<string> options;

    public Task StartAsync(IDialogContext context)
    {
        context.Wait(this.MessageReceivedAsync);
        return Task.CompletedTask; 
    }

    public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
    {
        var message = await result;
        options = new List<string> { CarouselCard, HeroCard, ThumbnailCard, ReceiptCard, SigninCard, VideoCard, AudioCard };

        PromptDialog.Choice<string>(
            context,
            this.DisplaySelectedCard,
            this.options,
            "What card would like to test?",
            "Ooops, what you wrote is not a valid option, please try again",
            3,
            PromptStyle.Keyboard);
    }
    
    public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
    {
        var selectedCard = await result;

        var message = context.MakeMessage();
        List<Attachment> attachment = null;

        if ((attachment = GetSelectedCard(selectedCard)) != null) 
        {
            message.Attachments = attachment;
            if (attachment.Count > 1)
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            
            await context.PostAsync(message);
            options.RemoveAll(x => x == selectedCard);
        }
        
        if (options.Count > 0)
            PromptDialog.Choice<string>(
                context,
                this.DisplaySelectedCard,
                this.options,
                $"Keep going! I have {options.Count} more for you to see.",
                "Ooops! What you wrote is not a valid option. Please try again.",
                3,
                PromptStyle.Keyboard);
        else
        {
            await context.PostAsync("You tested everything! If you want to start over type anything.");
            context.Wait(this.MessageReceivedAsync);
        }
    }

    private static List<Attachment> GetSelectedCard(string selectedCard)
    {
        switch (selectedCard)
        {
            case HeroCard:
                return new List<Attachment>() { GetHeroCard() }; 
            case ThumbnailCard:
                return new List<Attachment>() { GetThumbnailCard() };
            case ReceiptCard:
                return new List<Attachment>() { GetReceiptCard() };
            case SigninCard:
                return new List<Attachment>() { GetSigninCard() };
            case VideoCard:
                return new List<Attachment>() { GetVideoCard() };
            case AudioCard:
                return new List<Attachment>() { GetAudioCard() };
            case CarouselCard:
                return new List<Attachment>() {
                    GetHeroCard(), GetThumbnailCard(), GetAudioCard(), GetVideoCard()
                };

            default:
                return new List<Attachment>();
        }
    }

    private static Attachment GetHeroCard()
    {
        var heroCard = new HeroCard
        {
            Title = "Marczak.IO Azure Bot Series Hero Card",
            Subtitle = "Smarter bots with natural language processing",
            Text = "The closer bot interaction gets to the one of a human the better the end user experience will be. See how to leverage Microsoft Cognitive Services LUIS for natural language processing so that users can type naturally while allowing bots to understand and act.",
            Images = new List<CardImage> { new CardImage("https://marczak.io/images/botseries-luis/splash.svg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Read Article", value: "https://marczak.io/posts/botseries-smarter-bots-with-nlp/") }
        };

        return heroCard.ToAttachment();
    }

    private static Attachment GetThumbnailCard()
    {
        var heroCard = new ThumbnailCard
        {
            Title = "Marczak.IO Azure Bot Series Thumbnail Card",
            Subtitle = "Precompiled Bots with VS 2017 tooling",
            Text = "With the release of Visual Studio 2017 15.3 developers can take advantage of new Tools for Azure Functions. Learn how to take advantage of those tools to deliver pre-compiled bots for Azure Bot Service.",
            Images = new List<CardImage> { new CardImage("https://marczak.io/images/compilable-bots/splash.svg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Read Article", value: "https://marczak.io/posts/precompiled-bots-for-bot-service/") }
        };

        return heroCard.ToAttachment();
    }

    private static Attachment GetReceiptCard()
    {
        var receiptCard = new ReceiptCard
        {
            Title = "Adam Marczak",
            Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 1234-****") },
            Items = new List<ReceiptItem>
            {
                new ReceiptItem("Func App", price: "$ 11.21", quantity: "16", image: new CardImage(url: "https://marczak.io/images/symbols/functions.svg")),
                new ReceiptItem("VS License", price: "$ 450.00", quantity: "1", image: new CardImage(url: "https://marczak.io/images/symbols/vs17.svg")),
            },
            Tax = "$ 7.50",
            Total = "$ 468.71",
            Buttons = new List<CardAction>
            {
                new CardAction(
                    ActionTypes.OpenUrl,
                    "More information",
                    "https://marczak.io/images/symbols/azure.svg",
                    "https://azure.microsoft.com/en-us/pricing/")
            }
        };

        return receiptCard.ToAttachment();
    }

    private static Attachment GetSigninCard()
    {
        var signinCard = new SigninCard
        {
            Text = "MarczakIO - Azure Sign-In Card",
            Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
        };

        return signinCard.ToAttachment();
    }

    private static Attachment GetVideoCard()
    {
        var videoCard = new VideoCard
        {
            Title = "MarczakIO Azure Bot Series Video Card",
            Subtitle = "Introduction to chatbots",
            Text = "In recent years increased availability of internet across the world and evolution of mobile devices popularized messaging applications which became most common way to communicate in private life as well as everyday business. It is no surprise that leveraging those communication channels has huge potential in any business..",
            Image = new ThumbnailUrl
            {
                Url = "https://marczak.io/images/botseries-intro/splash.svg"
            },
            Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://marczak.io/images/botseries-rich-cards/CreatingBot.mp4"  
                }
            },
            Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Read Article",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://marczak.io/posts/botseries-introductiontobots/"
                }
            }
        };

        return videoCard.ToAttachment();
    }

    private static Attachment GetAudioCard()
    {
        var audioCard = new AudioCard
        {
            Title = "MarczakIO Azure Bot Series Audio Card",
            Subtitle = "Smarter bots with natural language processing",
            Text = "LUIS is shorthand name for Language Understanding Intelligent Service. It is one of Microsoft services available in Cognitive Services package in Azure. LUIS is very well described by a quote from Microsoft Azure website.",
            Image = new ThumbnailUrl
            {
                Url = "https://marczak.io/images/botseries-luis/splash.svg"
            },
            Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://marczak.io/images/botseries-rich-cards/SmarterBotsVoice.mp3"
                }
            },
            Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Read More",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://marczak.io/posts/botseries-smarter-bots-with-nlp/"
                }
            }
        };

        return audioCard.ToAttachment();
    }
}