using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using MakingBot.Card;
using PuppeteerSharp;
using SimpleWebScraper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakingBot.Commands
{
    internal class Utility : BaseCommandModule
    {
        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello, {ctx.User.Username}.");
        }

        [Command("sum")]
        public async Task sumCommand(CommandContext ctx, int number1, int number2)
        {
            int sum = number1 + number2;
            await ctx.Channel.SendMessageAsync(sum.ToString());
        }

        [Command("minus")]
        public async Task minusCommand(CommandContext ctx, int number1, int number2)
        {
            int minus = number1 - number2;
            await ctx.Channel.SendMessageAsync(minus.ToString());
        }

        [Command("embed")]
        public async Task embedCommand(CommandContext ctx)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = "Yotsuba Koiwai",
                Description = "Protagonist of the manga \"Yotsuba&!\"",
                Color = DiscordColor.SpringGreen,
                ImageUrl = "https://www.siliconera.com/wp-content/uploads/2024/12/new-yotsuba-manga-volume-16-finally-debuts-in-2025.png",
                Timestamp = DateTime.Now,
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("drawcard")]
        public async Task CardGame(CommandContext ctx)
        {
            CardSystem userCard = new();

            DiscordEmbedBuilder userCardEmbed = new()
            {
                Title = $"{ctx.User.Username} drew {userCard.SelectedNumber} of {userCard.SelectedSuit}",
                Color = userCard.EmbedColor,
            };

            await ctx.Channel.SendMessageAsync(embed: userCardEmbed);

            CardSystem botCard = new();

            DiscordEmbedBuilder botCardEmbed = new()
            {
                Title = $"Bot drew a {botCard.SelectedNumber} of {botCard.SelectedSuit}",
                Color = botCard.EmbedColor,
            };

            await ctx.Channel.SendMessageAsync(embed: botCardEmbed);

            DiscordEmbedBuilder resultMessage;

            if ((int)userCard.SelectedNumber > (int)botCard.SelectedNumber)
            {
                resultMessage = new()
                {
                    Title = $"{ctx.User.Username} wins.",
                    Color = DiscordColor.Green,
                };
            }
            else if (((int)userCard.SelectedNumber == (int)botCard.SelectedNumber))
            {
                resultMessage = new()
                {
                    Title = $"It's a draw.",
                    Color = DiscordColor.DarkBlue,
                };
            }
            else
            {
                resultMessage = new()
                {
                    Title = $"The bot wins.",
                    Color = DiscordColor.Red,
                };
            }

            await ctx.Channel.SendMessageAsync(embed: resultMessage);
        }

        [Command("intertest")]
        public async Task TestingInterac(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToReceive = await interactivity.WaitForMessageAsync(message => message.Content.ToLower().Trim() == "hello");

            if (messageToReceive.Result.Content.ToLower().Trim() == "hello")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} SAID HELLO");
            }
        }

        [Command("intertest2")]
        public async Task ReactingInterac(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToReact = await interactivity.WaitForReactionAsync(message => message.Message.Id == 1352682204010123365);
            if (messageToReact.Result.Message.Id == 1352682204010123365)
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} reacted to a message with {messageToReact.Result.Emoji}");
            }

        }

        [Command("poll")]
        public async Task MakePoll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(10);


            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(Program.Client, ":one:"),
                DiscordEmoji.FromName(Program.Client, ":two:"),
                DiscordEmoji.FromName(Program.Client, ":three:"),
                DiscordEmoji.FromName(Program.Client, ":four:"),

            };

            string optionsDescription = $@"{emojiOptions[0]} | {option1}.
{emojiOptions[1]} | {option2}.
{emojiOptions[2]} | {option3}.
{emojiOptions[3]} | {option4}.";

            DiscordEmbedBuilder pollMessage = new()
            {
                Title = pollTitle,
                Description = optionsDescription,
                Color = DiscordColor.MidnightBlue,
                Timestamp = DateTime.Now,
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(embed: pollMessage);
            foreach (var emoji in emojiOptions)
            {
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll, pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0])
                    count1++;
                if (emoji.Emoji == emojiOptions[1])
                    count2++;
                if (emoji.Emoji == emojiOptions[2])
                    count3++;
                if (emoji.Emoji == emojiOptions[3])
                    count4++;
            }

            int totalvotes = count1 + count2 + count3 + count4;

            string resultsDescription = $@"{option1} | {count1}
{option2} | {count2}
{option3} | {count3}
{option4} | {count4}
Total Votes: {totalvotes}";

            DiscordEmbedBuilder resultsMessage = new()
            {
                Title = "Results of the poll.",
                Description = resultsDescription,
                Color = DiscordColor.MidnightBlue,
                Timestamp = DateTime.Now,
            };

            await ctx.Channel.SendMessageAsync(embed: resultsMessage);
        }

        [Command("price")]
        public async Task findSales(CommandContext ctx, string lang, params string[] gameWords)
        {
            //await new BrowserFetcher().DownloadAsync(0);

            string game = string.Join(' ', gameWords);

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                DefaultViewport = new ViewPortOptions() { Width = 1280 },
                Headless = true
            });

            var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            await page.GoToAsync($"https://gg.deals/search/?title={game}", timeout: 0);

            await ChangeLanguage(ctx, page, lang);


            try
            {
                await GetGameList(ctx, page);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await GetStorePrices(ctx, page);

            await browser.CloseAsync();
        }

        static async Task ChangeLanguage(CommandContext ctx, IPage page, string lang)
        {
            var languageToSearch = lang.ToLower().Trim() switch
            {
                "au" => "Australia",
                "be" => "Belgium",
                "ca" => "Canada",
                "dk" => "Denmark",
                "eu" => "Europe",
                "fi" => "Finland",
                "fr" => "France",
                "de" => "Germany",
                "ie" => "Ireland",
                "it" => "Italy",
                "ne" => "Netherlands",
                "no" => "Norway",
                "pl" => "Poland",
                "es" => "Spain",
                "se" => "Sweden",
                "ch" => "Switzerland",
                "uk" => "United Kingdom",
                "us" => "United States",
                _ => ""
            };


            var regionList = await page.QuerySelectorAsync("#settings-menu-region");
            var regionButton = await regionList.QuerySelectorAsync(".settings-menu-select-action");

            await regionButton.ClickAsync();

            var languageList = await regionList.QuerySelectorAllAsync(".settings-menu-select-option");

            foreach (var language in languageList)
            {
                var languageButton = await language.QuerySelectorAsync(".region-change-link");

                string languageText = await (await languageButton.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                if (languageText.Trim() == languageToSearch)
                {
                    await ctx.Channel.SendMessageAsync($"Language changed to {lang.ToUpper()}.");
                    await languageButton.ClickAsync();
                    //wait for language change before swapping pages, otherwise net::ERR_ABORT will occur
                    await page.WaitForNavigationAsync();
                    break;
                }

            }
        }
        static async Task GetGameList(CommandContext ctx, IPage page)
        {
            var games = await page.QuerySelectorAllAsync(".game-item");
            StringBuilder finalMessage = new();

            List<Game> gameSelection = new();

            foreach (var game in games)
            {
                var gameTitleElement = await game.QuerySelectorAsync(".title-inner");

                string gameTitle = await (await gameTitleElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                var link = await game.QuerySelectorAsync(".full-link");

                string linkText = await (await link.GetPropertyAsync("href")).JsonValueAsync<string>();

                gameSelection.Add(new Game(gameTitle, linkText));
            }
            var options = new List<DiscordSelectComponentOption>();
            for (int i = 0; i < gameSelection.Count; i++)
            {
                finalMessage.AppendLine($"{i + 1}. {gameSelection[i].Name}");
            }


            DiscordEmbedBuilder message = new()
            {
                Title = "Results",
                Description = finalMessage.ToString(),
                Color = DiscordColor.SpringGreen,
                ImageUrl = "https://www.siliconera.com/wp-content/uploads/2024/12/new-yotsuba-manga-volume-16-finally-debuts-in-2025.png",
                Timestamp = DateTime.Now,
            };

            var interactivity = ctx.Client.GetInteractivity();
            var pages = interactivity.GeneratePagesInEmbed(finalMessage.ToString(), DSharpPlus.Interactivity.Enums.SplitType.Line);

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);

            int index = 0;

            try
            {
                var selectOptions = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption(
                "Label, no description",
                "label_no_desc"),

                new DiscordSelectComponentOption(
                "Label, Description",
                "label_no_desc",
                "This is a description"),

                new DiscordSelectComponentOption(
                "Label, Description, Emoji",
                "label_with_desc_emoji",
                "This is a description!",
                emoji: new DiscordComponentEmoji(854260064906117121)),

                new DiscordSelectComponentOption(
                "Label, Description, Emoji (Default)",
                "label_with_desc_emoji_default",
                "This is a description!",
                isDefault: true,
                new DiscordComponentEmoji(854260064906117121))
            };

                var dropdown = new DiscordSelectComponent("dropdown", null, options, false, 1, 2);

                var builder = new DiscordMessageBuilder().WithContent("Look, it's a dropdown!").AddComponents(dropdown);

                await builder.SendAsync(ctx.Channel);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            while (true)
            {
                await ctx.Channel.SendMessageAsync("Pick an Option: ");
                var selectedGame = await interactivity.WaitForMessageAsync(message => message.Author == ctx.User);

                if (!Int32.TryParse(selectedGame.Result.Content, out index))
                {
                    Console.WriteLine(selectedGame.Result.Content);
                    await ctx.Channel.SendMessageAsync("Wrong input.");
                }
                if (index > gameSelection.Count)
                {
                    await ctx.Channel.SendMessageAsync("Invalid index.");
                }
                break;
            }

            await page.GoToAsync(gameSelection[index - 1].Link);
        }

        static async Task GetStorePrices(CommandContext ctx, IPage page)
        {
            var interactivity = ctx.Client.GetInteractivity();
            List<DSharpPlus.Interactivity.Page> pages = new();
            var showMoreButton = await page.QuerySelectorAllAsync(".btn-game-see-more");

            foreach (var button in showMoreButton)
            {
                await button.ClickAsync(new PuppeteerSharp.Input.ClickOptions() { Count = 2 });
                Thread.Sleep(2000);
            }

            var games = await page.QuerySelectorAllAsync(".game-list-item");

            Console.WriteLine(games.Length);

            int pageCount = 0;
            int currentPage = 0;

            foreach (var game in games)
            {
                var type = await game.EvaluateFunctionAsync<string>("game => game.getAttribute('data-shop-name')");
                if (!string.IsNullOrWhiteSpace(type))
                    pageCount++;
            }


            for (int i = 0; i < games.Length; i++)
            {
                var type = await games[i].EvaluateFunctionAsync<string>("game => game.getAttribute('data-shop-name')");
                //excludes stuff like dlc, packs, etc.
                if (!string.IsNullOrWhiteSpace(type))
                {
                    currentPage++;
                    Console.WriteLine("thingabob");
                    var priceElement = await games[i].QuerySelectorAsync(".price-inner");

                    priceElement ??= await games[i].QuerySelectorAsync(".price-text");

                    string price = await (await priceElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                    var linkElement = await games[i].QuerySelectorAsync(".full-link");

                    string link = await (await linkElement.GetPropertyAsync("href")).JsonValueAsync<string>();

                    var gameTitleEl = await games[i].QuerySelectorAsync(".game-info-title");

                    string gameTitle = await (await gameTitleEl.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                    var embedFooter = new DiscordEmbedBuilder.EmbedFooter();
                    embedFooter.Text = $"Page: {currentPage}/{pageCount}";

                    DiscordEmbedBuilder message = new DiscordEmbedBuilder()
                    {
                        Title = $"{gameTitle}",
                        Description = $"Store: {type}\nPrice: {price}\n[Link]({link})",
                        Footer = embedFooter
                    };

                    DSharpPlus.Interactivity.Page pageOfPrices = new(embed: message);

                    pages.Add(pageOfPrices);
                }
            }

            Console.WriteLine(pages.Count);
            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
        }
    }
}